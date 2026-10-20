import { usePack } from "@/entities/pack/api/pack.queries.ts";
import { useParams } from "react-router-dom";
import PackEditor from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";
import Header from "@/widgets/Header/Header.tsx";
import styles from "./Pack.module.css"

const Pack = () => {
  const { id } = useParams<{id : string}>();
  const { data } = usePack(id);
  return (
    <div className={styles.page}>
      <Header showAuthButtons={false} />
      <PackEditor packId={id} initial={data} />
    </div>
  );
};

export default Pack;