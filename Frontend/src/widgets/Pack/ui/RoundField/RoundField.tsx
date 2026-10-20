import styles from "./RoundField.module.css"
import { AddItem } from "@/shared/ui/AddItem/AddItem.tsx";
import CellTable from "@/shared/ui/CellTable/CellTable.tsx";
import { usePackDraftContext } from "@/widgets/Pack/model/PackDraftContext.tsx";

const RoundField = ( { idx } : { idx : number }) => {
  const draft = usePackDraftContext();
  const round = draft.pack.rounds[idx];

  return (
      <div className={styles.field}>
        <table className={styles.gameTable}>
          <thead>
            <tr>
              <th className={styles.cornerCell}>Категория / Цена</th>
              {round.costs.map((cost, cIdx) => (
                <th key={cIdx} className={styles.costTh}>
                  <div className={styles.thContent}>
                    <span>{cost}</span>
                  </div>
                </th>
              ))}
              <th>
                <AddItem label="Добавить стоимость" inputType="number" onSubmit={(value) => draft.addCost(Number(value))}/>
              </th>
            </tr>
          </thead>
          <tbody>
          {round.categories.map((category, row) => (
            <tr key={row}>
              <td className={styles.catTd}>
                <div className={styles.tdContent}>
                  <span>{category}</span>
                </div>
              </td>

              {round.costs.map((cost, col) => {
                return (
                  <td key={col}>
                    <CellTable
                      cell={draft.pack.rounds[idx].questions[row]?.[col] ?? null}
                      cost={cost}
                      category={category}
                      onCellDrop={(cell) => draft.setCell(row, col, cell)}
                    />
                  </td>
                );
              })}
            </tr>
          ))}
          <tr>
            <td>
              <AddItem label="Добавить категорию" onSubmit={draft.addCategory} />
            </td>
          </tr>
          </tbody>
        </table>
    </div>
  );
};

export default RoundField;