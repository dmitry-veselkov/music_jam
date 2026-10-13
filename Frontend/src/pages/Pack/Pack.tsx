import { usePack } from "@/entities/pack/api/pack.queries.ts";
import { useParams } from "react-router-dom";
import PackEditor from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";

const Pack = () => {
  const { id } = useParams<{id : string}>();
  const { data } = usePack(id);
  return <PackEditor packId={id} initial={data} />;
};

export default Pack;