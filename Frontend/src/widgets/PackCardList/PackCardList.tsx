import { PackCard, usePacks } from "@/entities/pack";
import styles from "./PackCardList.module.css";

const PackCardList = () => {
  const { data } = usePacks();
  const items = data?.packs ?? [];
  console.log(items.length > 0 ? typeof items[0].creationDate : "рендер епта");
  return (
    <div>
      <ul className={styles.packList}>
        {items.map((pack) => (
          <PackCard key={pack.packId} {...pack}  />
        ))}
      </ul>
    </div>
  );
};

export default PackCardList;