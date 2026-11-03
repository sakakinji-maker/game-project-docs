# 3스테이지 보스 — 태양 봉인의 집행관 아우렐

## 진입과 구성

대성당 진입로 → 왕실 회랑 → 태양 봉인 예배당의 수비대를 모두 처치하면 내전으로 가는 문이 열립니다. 지상에서 문 앞 ↑로 네 번째 방인 **태양 봉인의 내전**에 진입합니다. 중앙 X=32, Y=0에 보스가 등장합니다. HP 600, 지상 이동 범위 X=8~56, 몸체 반폭 1.2·높이 3.9입니다.

내전의 발판은 (8,9,1.6), (20,7,1.6), (37,7,1.6), (47,9,1.6)입니다. 가운데 지면은 열어 두고 낮은 발판으로 태양 베기를 피할 수 있게 했습니다. 예배당의 석조 창문/기둥에 태양 봉인 문장과 사슬을 더했습니다.

1·2보스에서 얻은 스킬과 현재 HP·장비·성장을 유지합니다. 사슬 스킬도 보스에게 피해를 줄 수 있으며 포박 사슬은 기존 규칙대로 보스를 강제 이동시키지 않습니다.

## 공격 패턴

| 패턴 | 1페이즈 | 2페이즈 | 대응 |
|---|---|---|---|
| 태양 베기 | 예고 0.85초, 좌우 8, 피해 24 | 예고 0.65초, 좌우 10 | 바닥 표시 밖으로 이동하거나 발 높이 1.2 이상으로 점프/발판 대피 |
| 왕실 광창 | 예고 1.05초, 3개 | 예고 0.85초, 5개 | 시전 시작 때 고정된 세로 기둥 사이로 이동. 기둥 반폭 0.7, 간격 4, 높이 9, 피해 18 |
| 흑일 심판 | 예고 1.6초, 안전 반폭 3 | 예고 1.3초, 안전 반폭 2.4 | 초록빛 세로 안전지대에 몸 전체가 들어가야 함. 밖은 높이와 무관하게 피해 28 |

공격은 순서대로 반복합니다. 태양 베기 사거리 밖이면 광창으로 대체하므로 멀리 있어도 공격을 계속합니다. 흑일 안전지대는 플레이어 위치에서 보스가 바라보는 방향으로 5 떨어진 지점에 생성하며 X=5~59로 제한합니다. 예고 후 추적하지 않고, 화면 밖이면 방향 안내를 표시합니다. 이동과 구르기 무적 모두로 대응할 수 있습니다. 판정은 예고 종료 시 한 번만 일어나고 남은 효과는 잔상입니다.

체력 300 이하에서 진행 중인 공격을 취소하고 1.2초간 2페이즈를 알립니다. 붉어진 색조, 이동 속도 2→2.7, 넓어진 베기, 늘어난 광창, 좁아진 안전지대를 적용합니다. 공격 후 대기 1.5→1.1초이며 보스 처치 이후 공격 판정은 남지 않습니다.

## 이야기와 처치

등장 6쪽 / 처치 7쪽. 전체 대본은 STORY_DIALOGUE.md 하단에 기록합니다.

아우렐은 양어머니가 왕실 봉인의 마지막 매듭이 되었음을 밝힙니다. 양어머니는 다른 이들을 보호하려고 새어 나오는 빛과 균열을 버티고 있습니다. 주인공은 양어머니를 대가로 삼으려는 왕실의 명령을 거부하고, 사슬 대신 별실로 틈을 수선하려 합니다.

등장 연출 1.8초, 처치 연출 0.35초에 대사가 열리고 Space/다음 버튼으로 진행합니다. 대사 중 전투·연출·카메라는 멈추며 기존 장별 일회 표시 규칙을 사용합니다. 양어머니의 화자 이름은 '봉인 너머의 목소리'로 표시합니다.

처치 후 사슬이 풀리고 중앙 태양 문장이 밝아지며 금빛 봉합 흔적이 남습니다. 기존 붉은 입자 소멸, 2초 후 포탈, 2.5초 후 상자, 3.2초 후 조작 재개를 연결했습니다. 처치 보상은 골드 350 / 경험치 220으로 한 번만 지급합니다. 상자는 HP 40 즉시 회복과 미보유 장비 1종이며 보유한 장비는 중복 지급하지 않습니다.

1·2보스 보상 스킬은 재추첨하지 않고 그대로 보존합니다. 아우렐 처치 시 별도 태양 스킬(태양의 창 / 여명의 폭발) 하나를 무작위 해금하고 처치 연출과 획득 안내에 표시합니다. 상세: SOLAR_SKILLS.md. 포탈 앞 ↑로 4스테이지 왕관의 심장부에 진입합니다(CHAPTER_FOUR.md). 현재는 봉인 **외곽**을 해방하고 양어머니의 목소리를 되찾은 단계이며, 안쪽 매듭 해제와 최종 구출 결말은 아직 구현하지 않았습니다.

## 미술

- 내장 image_gen 모드, 투명 배경. 기존 모르가 도트를 계열/화풍 참고로 신규 생성한 뒤 여백을 보완하는 편집 1회.
- 적용: Assets/Sunfall/Resources/Demons/Stage3SolarRegentV1.png (1774×887).
- 선택 출력: C:/Users/sakak/.codex/generated_images/01a17569-bfbf-7233-a71a-72eece5bde92/exec-cb3a59e3-9eb6-46eb-9da6-2dffccf9ab09.png
- 최초 출력: 같은 폴더의 exec-d8bbdbeb-5991-4fac-b2c3-42e9ad3d94cc.png. 캐릭터/무기 여백을 확보한 편집본을 프로젝트에 복사해 사용합니다.
- 4×2 시트: 대기 2 / 걷기 2 / 베기 준비 / 베기 타격 / 광창 시전 / 흑일 시전. 전 프레임 같은 배율 0.42와 발 기준점, 포즈 보간 0.09초. 알파 보존, Point/Clamp/무압축/밉맵 없음. 가져오기 동안 기존 모르가 도트로 대체 표시합니다.
- 공격 예고, 광창, 태양 봉인과 봉합 흔적은 코드로 그립니다.

최초 생성 프롬프트:

```text
Use case: stylized-concept. Create a NEW transparent pixel-art boss sprite sheet for a dark gothic 2D side-scrolling game. Reference image is only a style/family reference, NOT an edit target. Character: Aurel, an adult royal demon high priest guarding the stolen sun. Distinct from the reference: tall lean figure, obsidian and ivory clerical plate armour, tattered white mantle with muted crimson lining, angular horned golden solar crown, dark face with ember eyes, two folded bat wings, long polearm with a crescent sun blade. Fully armoured, nonsexual. A small broken sun halo behind head. Elegant menacing royal executioner. Crisp game pixel art, limited charcoal, antique gold, ivory and crimson palette, not photorealistic. Exact 4 columns x 2 rows grid, eight equally sized cells, generous transparent gutters. No text, no numbers, no background, no floor, no shadow, true transparency. All eight characters face RIGHT and use identical head/torso anatomical size and heel baseline within cell, no zoom. Top row: idle ready, idle breath, walking step left, walking step right. Bottom row: weapon raised for sweep windup, low horizontal sweep follow-through, forward hand conjuring solar lances, both arms raised invoking an eclipse. Keep whole weapon, crown, wings and feet INSIDE each cell with padding. No large magic beams or trails, effects rendered separately in game. Landscape sheet.
```

여백 편집 프롬프트:

```text
Edit this sprite sheet only to fix layout and padding. Preserve the same Aurel character design, same 8 poses, colors and pixel-art rendering. Recompose into a strict 4 columns x 2 rows equal grid. Uniformly reduce EVERY complete character including weapon, wings and crown to 75 percent of its present size (same scale factor for all frames). Center each complete pose inside its own cell, align heels to the same relative baseline in each row. Leave at least 30 transparent pixels between every figure and all four edges of its cell; no overlap, no cropped weapons or wings, including the rightmost column. Keep true transparent background, no text or lines. Landscape 2:1 image. Preserve relative anatomical size across all frames. This is a sprite atlas for a game, gutters and fully-contained poses are essential.
```

## 확인 범위

Core / Runtime / Editor / Tests / 독립 CoreChecks 컴파일만 확인합니다. 로그: Temp/SolarBossCompile/compile.log.

새 보스 진입, 이전 스킬 4가지 조합의 피해 경로, 지상/공중 베기 회피, 광창의 고정 목표와 기둥 간격, 흑일 안전지대와 구르기 무적, 절반 체력 전환, 보상 중복 방지, 연출·보스 생존 중 출구 잠금, 상자와 최종 포탈 조건에 대한 테스트 코드를 추가했습니다. 사용자 요청대로 테스트 실행·Play·게임 QA·플레이어 빌드는 수행하지 않았습니다.

