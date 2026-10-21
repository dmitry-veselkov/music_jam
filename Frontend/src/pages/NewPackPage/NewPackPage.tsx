import { useState } from "react";
import { PackEditorForm } from "@/widgets/Pack/ui/PackEditor/PackEditor.tsx";

const NewPackPage = () => {
  const [packId] = useState(() => crypto.randomUUID());
  return <PackEditorForm packId={packId} />;
};

export default NewPackPage;