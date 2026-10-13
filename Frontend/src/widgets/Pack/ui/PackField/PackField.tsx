import styles from "./PackField.module.css"
import { Button } from "@/shared/ui/Button/Button.tsx";
import RoundField from "@/widgets/Pack/ui/RoundField/RoundField.tsx";
import PackQuestionsSection from "@/widgets/Pack/ui/PackQuestionsSection/PackQuestionsSection.tsx";
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";

const PackField = () => {
  const draft  = usePackDraftContext();

  return (
    <div className={styles.page}>
      <div className={styles.questionSection}>
        <PackQuestionsSection />
      </div>
      <div className={styles.round}>
        <div className={styles.roundSwitcher}>
          <div className={styles.roundTitles}>
            {draft.pack.rounds.map((round, index) => (
              <Button
                key={index}
                type="button"
                onClick={() => draft.setRoundIndex(index)}
                text={round.name}
              />
            ))}
          </div>
          <div className={styles.addRound}>
            <AddItem label={"Добавить раунд"} onSubmit={draft.addRound} />
          </div>
        </div>
        <RoundField idx={draft.roundIndex}/>
      </div>
    </div>
  );
};

export default PackField;