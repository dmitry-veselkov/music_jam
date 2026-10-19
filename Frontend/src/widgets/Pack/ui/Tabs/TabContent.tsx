import QuestionCreator from "@/widgets/QuestionCreator/QuestionCreator.tsx";
import PackField from "@/widgets/Pack/ui/PackField/PackField.tsx";
import type { Tab } from "@/widgets/Pack/model/types.ts";

const TabContent = ({ active } : {active : Tab}) => {
  if (active === 'questions') return <QuestionCreator />;
  if (active === 'field') return <PackField />;
  return null;
};

export default TabContent;