import type { PackDto } from "@/entities/pack/models/types.ts";
import { usePackDraft } from "@/widgets/Pack/model/usePackDraft.ts";
import { useUpdatePack } from "@/features/pack/api/useUpdatePack.ts";
import { useCreatePack } from "@/features/pack/api/useCreatePack.ts";
import { useState } from "react";
import type { Tab } from "@/widgets/Pack/model/types.ts";
import { PackDraftProvider } from "@/widgets/Pack/model/PackDraftContext.tsx";
import Input from "@/shared/ui/Input/Input.tsx";
import TabsNav from "@/widgets/Pack/ui/Tabs/TabsNav.tsx";
import TabContent from "@/widgets/Pack/ui/Tabs/TabContent.tsx";
import styles from "./PackEditor.module.css"
import { Button } from "@/shared/ui/Button/Button.tsx";
import { usePack } from "@/entities/pack/api/pack.queries.ts";

const PackEditor = ({ packId, initial } : { packId? : string, initial? : PackDto}) => {
  const { data } = usePack(packId);
  const source = initial ?? data;
  const draft = usePackDraft(source);

  const [activeTab, setActiveTab] = useState<Tab>('questions');
  const [nameError, setNameError] = useState('');

  const create = useCreatePack();
  const update = useUpdatePack();

  const onSave = () => {
    if (!draft.pack.name.trim()) return setNameError('Введите название пака');
    if (initial) update.mutate({ id: initial.packId, ...draft.pack });
    else create.mutate(draft.pack);
  };

  return (
    <PackDraftProvider draft={draft}>
      <div className={styles.page}>
        <div className={styles.title}>
          <Input value={draft.pack.name} onChange={(e) => draft.setName(e.target.value)} />
        </div>
        <div className={styles.options}>
          <TabsNav active={activeTab} onChange={setActiveTab} />
          <div className={styles.packSave}>
            <Button text={initial ? 'Сохранить' : 'Создать'} onClick={onSave} disabled={create.isPending || update.isPending} />
          </div>
        </div>
        <TabContent active={activeTab} />
        {nameError && <span>{nameError}</span>}
      </div>
    </PackDraftProvider>
  );
};

export default PackEditor;