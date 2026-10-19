import type { PackDto } from "@/entities/pack/models/types.ts";
import { usePackDraft } from "@/widgets/Pack/model/usePackDraft.ts";
import { useUpsertPack } from "@/features/pack/api/useUpsertPack.ts";
import { useState } from "react";
import type { Tab } from "@/widgets/Pack/model/types.ts";
import { PackDraftProvider } from "@/widgets/Pack/model/PackDraftContext.tsx";
import Input from "@/shared/ui/Input/Input.tsx";
import TabsNav from "@/widgets/Pack/ui/Tabs/TabsNav.tsx";
import TabContent from "@/widgets/Pack/ui/Tabs/TabContent.tsx";
import styles from "./PackEditor.module.css"
import { Button } from "@/shared/ui/Button/Button.tsx";
import { usePack } from "@/entities/pack/api/pack.queries.ts";
import { packSchema } from "@/features/pack/model/schema.ts";

const PackEditor = ({ packId, initial } : { packId? : string, initial? : PackDto}) => {
  const { data, isError } = usePack(packId);
  const source = initial ?? data;

  if (isError) return <div>Не удалось загрузить пак</div>;

  return <PackEditorForm key={source?.packId ?? 'new'} initial={source} />;
};

const PackEditorForm = ({ initial }: { initial?: PackDto }) => {
  const draft = usePackDraft(initial);
  const [activeTab, setActiveTab] = useState<Tab>('questions');
  const [error, setError] = useState('');

  const upsert = useUpsertPack();
  console.log(error);

  const onSave = () => {
    const parsed = packSchema.safeParse(draft.pack);
    if (!parsed.success) return setError(parsed.error.issues[0].message);

    setError('');
    upsert.mutate({ id: draft.packId, ...parsed.data });
  };

  const isNew = !initial;
  return (
    <PackDraftProvider draft={draft}>
      <div className={styles.page}>
        <div className={styles.options}>
          <TabsNav active={activeTab} onChange={setActiveTab} />
          <div className={styles.title}>
            <Input
              value={draft.pack.name}
              onChange={(e) => {
                draft.setName(e.target.value);
              }}
            />
          </div>
          <div className={styles.packSave}>
            <Button text={isNew ? 'Создать' : 'Сохранить'} onClick={onSave} disabled={upsert.isPending} />
          </div>
        </div>
        <TabContent active={activeTab} />
      </div>
    </PackDraftProvider>
  );
};

export default PackEditor;