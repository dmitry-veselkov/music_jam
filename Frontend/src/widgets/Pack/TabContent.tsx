import PackQuestions from "@/widgets/Pack/PackQuestions/PackQuestions.tsx";
import PackField from "@/widgets/Pack/PackField/PackField.tsx";
import type { Tab } from "@/widgets/Pack/types.ts";

const TabContent = ({ active } : {active : Tab}) => {
  if (active === 'questions') return <PackQuestions />;
  if (active === 'field') return <PackField />;
  return null;
};

export default TabContent;