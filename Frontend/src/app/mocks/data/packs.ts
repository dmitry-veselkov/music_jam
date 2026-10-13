import type { PackDto } from "@/entities/pack/models/types.ts";

export const packs : PackDto[] = [
  {
    packId: "1",
    name: "Первый",
    creationDate: new Date('2026-04-15'),
    lastUpdateDate: new Date('2026-06-19'),
    rounds : [
      {
        name: "Первый",
        order : 1,
        categories: ["Первая"],
        costs: [100],
        questions : [
          [
            null
            // {
            //   cost: 100,
            //   category : "Первая",
            //   question :   {
            //     questionId: "1",
            //     requested: 'Название песни',
            //     correctAnswer: 'Смешарики песня',
            //     questionTrack: '',
            //     answerTrack: '',
            //   },
            // }
          ]
        ]
      },
    ]
  }
]
