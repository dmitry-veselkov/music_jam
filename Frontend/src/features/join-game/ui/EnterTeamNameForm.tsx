import { useState } from "react";

export const EnterTeamNameForm = ({ onSubmit }: { onSubmit: (name: string) => void }) => {
  const [value, setValue] = useState('');
  return (
    <form onSubmit={(e) => { e.preventDefault(); if (value.trim()) onSubmit(value.trim()); }}>
      <input
        value={value}
        onChange={(e) => setValue(e.target.value)}
        placeholder="Название команды"
        autoFocus
      />
      <button type="submit">Войти в зал</button>
    </form>
  );
};