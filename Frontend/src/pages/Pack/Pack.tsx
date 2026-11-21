import { useParams } from "react-router-dom";
import PackEditor from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";
import Header from "@/widgets/Header/ui/Header.tsx";
import styles from "./Pack.module.css"

const Pack = () => {
  const { id } = useParams<{id : string}>();
  return (
    <div className={styles.page}>
      <Header />
      <PackEditor packId={id!} />
    </div>
  );
};

export default Pack;