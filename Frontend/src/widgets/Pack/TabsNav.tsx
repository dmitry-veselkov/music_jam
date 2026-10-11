import type { Tab } from "@/widgets/Pack/types.ts";

const TabsNav = ({ active, onChange }: { active: Tab; onChange: (t: Tab) => void }) => {
  return (
    <div>
      <button disabled={active === 'questions'} onClick={() => onChange('questions')}><h1>Вопросы</h1></button>
      <button disabled={active === 'field'} onClick={() => onChange('field')}><h1>Поле</h1></button>
    </div>
  );
};

export default TabsNav;