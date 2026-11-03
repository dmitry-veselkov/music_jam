import { useParams } from "react-router-dom";
import PackEditor from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";
import Header from "@/widgets/Header/Header.tsx";
import styles from "./Pack.module.css"

const Pack = () => {
  const { id } = useParams<{id : string}>();
  return (
    <div className={styles.page}>
      <Header showAuthButtons={false} />
      <PackEditor packId={id!} />
    </div>
  );
};

export default Pack;