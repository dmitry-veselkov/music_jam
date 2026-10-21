import { useState } from "react";
import PackEditor from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";

const NewPackPage = () => {
  const [packId] = useState(() => crypto.randomUUID());
  return <PackEditor packId={packId} />;
};

export default NewPackPage;