import { useEffect, useState } from 'react';
import { useForm } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { type GameDto, MODES } from '@/entities/game/model/types.ts';
import type { PackShort } from '@/entities/pack';
import { gameSchema, type GameFormValues, type GameRequest } from '@/features/game';
import Input from '@/shared/ui/Input/Input.tsx';
import { Button } from '@/shared/ui/Button/Button.tsx';
import { useGame } from "@/entities/game/api/game.queries.ts";
import { usePackShorts } from "@/entities/pack/api/pack.queries.ts";
import { useUpsertGame } from "@/features/game";

const PAGE_SIZE = 12;

const toFormDate = (iso: string) => {
  const d = new Date(iso);
  const pad = (n: number) => String(n).padStart(2, '0');
  return {
    date: `${d.getFullYear()}-${pad(d.getMonth() + 1)}-${pad(d.getDate())}`,
    time: `${pad(d.getHours())}:${pad(d.getMinutes())}`,
  };
};

type GameEditorProps = {
  gameId?: string;
  onSaved?: () => void;
};

const GameEditor = ({ gameId, onSaved }: GameEditorProps) => {
  const { data: game, isError } = useGame(gameId);
  const [page, setPage] = useState(0);
  const { data : packs } = usePackShorts({ skip: page * PAGE_SIZE, take: PAGE_SIZE });
  const upsert = useUpsertGame({ onSuccess: onSaved });

  useEffect(() => {
    if (packs && packs.length === 0 && page > 0) setPage((p) => p - 1);
  }, [packs, page]);

  if (isError) return <div>Не удалось загрузить игру</div>;
  if (!packs || (gameId && !game)) return <div>Ошибка</div>;

  return (
    <GameForm
      key={game?.gameId ?? 'new'}
      game={game}
      packs={packs}
      onSave={upsert.mutate}
      isPending={upsert.isPending}
    />
  );
}

type GameFormProps = {
  game?: GameDto;
  packs: PackShort[];
  onSave: (data: GameRequest & { id: string }) => void;
  isPending?: boolean;
};

const GameForm = ({
  game,
  packs,
  onSave,
  isPending = false,
}: GameFormProps) => {
  const [gameId] = useState(() => game?.gameId ?? crypto.randomUUID());

  const { register, handleSubmit, formState: { errors } } = useForm<GameFormValues>({
    resolver: zodResolver(gameSchema),
    defaultValues: {
      packId: game?.pack.packId ?? '',
      name: game?.name ?? '',
      description: game?.description ?? '',
      ...(game ? toFormDate(game.date) : { date: '', time: '' }),
      mode: game?.mode ?? 'withHost',
    },
  });

  const onSubmit = (data: GameFormValues) => {
    console.log(data);
    onSave({
      id: gameId,
      packId: data.packId,
      name: data.name,
      description: data.description,
      date: new Date(`${data.date}T${data.time}`).toISOString(),
      mode: data.mode,
    });
  };

  return (
    <form onSubmit={handleSubmit(onSubmit)}>
      <div>
        <label htmlFor="game-name">Название игры</label>
        <Input
          id="game-name"
          placeholder="Название"
          {...register('name')}
        />
        {errors.name && <span>{errors.name.message}</span>}
      </div>

      <div>
        <label htmlFor="game-pack">Пак</label>
        <select id="game-pack" {...register('packId')}>
          <option value="">Выберите пак</option>
          {packs.map((pack) => (
            <option key={pack.packId} value={pack.packId}>
              {pack.name}
            </option>
          ))}
        </select>
        {errors.packId && <span>{errors.packId.message}</span>}
      </div>

      <div>
        <label htmlFor="game-description">Описание</label>
        <textarea
          id="game-description"
          placeholder="Описание игры"
          {...register('description')}
        />
        {errors.description && (
          <span>{errors.description.message}</span>
        )}
      </div>

      <div>
        <label htmlFor="game-date">Дата</label>
        <Input
          id="game-date"
          type="date"
          {...register('date')}
        />
        {errors.date && <span>{errors.date.message}</span>}
      </div>

      <div>
        <label htmlFor="game-time">Время</label>
        <Input
          id="game-time"
          type="time"
          {...register('time')}
        />
        {errors.time && <span>{errors.time.message}</span>}
      </div>

      <div>
        <label htmlFor="game-mode">Режим игры</label>
        <select id="game-mode" {...register('mode')}>
          {Object.values(MODES).map((m) => (
            <option key={m.value} value={m.value}>{m.label}</option>
          ))}
        </select>
      </div>

      <Button
        type="submit"
        disabled={isPending}
        text={game ? 'Сохранить игру' : 'Создать игру'}
        variant="primary"
      />
    </form>
  );
};

export default GameEditor;