import style from "./CellTable.module.css"
import { useState } from "react";
import type { DragEvent } from 'react';
import clsx from "clsx";
import type { CellDto } from "@/entities/pack/models/types.ts";
import type { QuestionDto } from "@/entities/question-form";

const EMPTY_TEXT = '-';

type CellTableProps = {
  cell : CellDto | null,
  cost: number;
  category: string;
  onCellDrop: (cell: CellDto) => void,
}

const CellTable = ({cell, cost, category, onCellDrop} : CellTableProps) => {
  const [isDragOver, setIsDragOver] = useState(false);

  const handleDragOver = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    e.dataTransfer.dropEffect = 'copy';
    setIsDragOver(true);
  };

  const handleDragLeave = (e: DragEvent<HTMLDivElement>) => {
    if (!e.currentTarget.contains(e.relatedTarget as Node | null)) {
      setIsDragOver(false);
    }
  };

  const handleDrop = (e: DragEvent<HTMLDivElement>) => {
    e.preventDefault();
    setIsDragOver(false);

    const data = e.dataTransfer.getData('application/json');
    if (!data) return;

    const question = JSON.parse(data) as QuestionDto;

    onCellDrop({
      cost,
      category,
      question,
    });
  };

  return (
    <div
      className={clsx(style.cell, cell && style.filled, isDragOver && style.dragOver)}
      title={cell?.question.correctAnswer ?? "Ячейка"}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      <span className={style.text}>{cell?.question?.correctAnswer ?? EMPTY_TEXT}</span>
    </div>
  );
};

export default CellTable;