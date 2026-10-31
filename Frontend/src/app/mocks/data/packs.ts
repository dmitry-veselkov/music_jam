import type { PackDto } from "@/entities/pack/model/types.ts";

export const packs : PackDto[] = [
  {
    packId: "1",
    name: "Первый",
    rounds : [
      {
        name: "Первый",
        categories: ["Первая"],
        costs: [100],
        questions : [
          [
            {
              cost: 100,
              category : "Первая",
              correctAnswer: 'Смешарики песня',
              questionId: "1",
            }
          ]
        ]
      },
    ]
  }
]
