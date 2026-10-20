import style from "./CellTable.module.css"
import { useState } from "react";
import type { DragEvent } from 'react';
import clsx from "clsx";
import type { QuestionDtoCard } from "@/entities/question";
import type { CellDraft } from "@/entities/pack/models/draft.ts";

const EMPTY_TEXT = '-';

type CellTableProps = {
  cost : number
  category : string
  cell: CellDraft | null;
  onCellDrop: (cell: CellDraft) => void,
}

const CellTable = ({cost, category, cell, onCellDrop} : CellTableProps) => {
  const [isDragOver, setIsDragOver] = useState(false);

  const isFilled = !!cell?.questionId;
  const answer = cell?.correctAnswer ?? null;

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

    const dropped = JSON.parse(data) as QuestionDtoCard;

    onCellDrop({
      cost,
      category,
      questionId: dropped.questionId,
      correctAnswer: dropped.correctAnswer ?? null,
    });
  };

  return (
    <div
      className={clsx(style.cell, isFilled && style.filled, isDragOver && style.dragOver)}
      title={answer ?? "Ячейка"}
      onDragOver={handleDragOver}
      onDragLeave={handleDragLeave}
      onDrop={handleDrop}
    >
      <span className={style.text}>{answer ?? EMPTY_TEXT}</span>
    </div>
  );
};

export default CellTable;