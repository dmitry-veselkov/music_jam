// import { useParams } from "react-router-dom";
import { useState } from "react";
import TabsNav from "@/widgets/Pack/TabsNav";
import TabContent from "@/widgets/Pack/TabContent";
import type { Tab } from "@/widgets/Pack/types.ts";

const Pack = () => {
  // const { id } = useParams<{id : string}>();
  const [activeTab, setActiveTab] = useState<Tab>('questions');

  return (
    <div>
      <TabsNav active={activeTab} onChange={setActiveTab} />
      <TabContent active={activeTab} />
    </div>
  );
};

export default Pack;