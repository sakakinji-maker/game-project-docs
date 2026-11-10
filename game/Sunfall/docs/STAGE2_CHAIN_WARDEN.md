2026-11-02 미술·등장 연출 최신 내용: [2층 회랑·모르가 개편](STAGE2_CRYPT_REDESIGN.md). 아래 기존 배경/외형 설명은 새 버전으로 대체되었습니다.

# 2스테이지 보스 — 왕실 사슬의 간수 모르가

## 구성

검은 성채의 적을 전부 처치하고 출구 앞 ↑로 왕실 사슬의 제단에 진입합니다. X=32 중앙에 뿔 달린 후드·검은 갑옷·왕실 문장·사슬 철퇴를 갖춘 지상 악마가 나타납니다. HP 420, 지상 이동 범위 X=8~56. 발판은 (9,8,1.6), (20,7,1.6), (37,7,1.6), (48,8,1.6)이며 가운데 지면은 비워두었습니다.

등장 3.6초 연출의 1.8초 시점에 대사가 열립니다. 대화가 끝나면 남은 연출 후 전투가 시작됩니다. 처치 대사는 처치 연출 0.35초에 열립니다. Space/다음 버튼으로 진행하고 대사 중 전투와 연출은 정지합니다. 장별 대사 상태를 따로 관리해 1보스 대화를 봤어도 2보스 대화가 나옵니다.

## 공격

| 패턴 | 기본 / 2페이즈 | 회피 방법 |
|---|---|---|
| 사슬 내려치기 | 0.8 / 0.65초 예고, 반경 2.5 / 3.2, 피해 22 | 고정된 바닥 표시 밖으로 이동하거나 발 높이 1.2 이상으로 점프·발판 대피 |
| 구속 사슬 | 1.05 / 0.85초 예고, 세로 기둥 1 / 3개, 피해 14 | 표시 밖으로 이동. 맞으면 0.7초간 이동 속도 40%, 구르기로 해제 |
| 왕실 사슬 낙하 | 1.15 / 0.95초 시전 뒤 기둥별 0.45초 이상 예고, 5 / 7갈래, 피해 18 | 높이 9까지 내려오는 사슬 사이의 틈으로 이동 |

- 체력이 210 이하가 되면 진행 중인 공격과 사슬을 취소하고 1.1초간 2페이즈를 알립니다. 붉은 색조, 이동 속도 2.1→2.8, 짧아진 예고와 늘어난 기둥 수를 적용합니다.
- 공격 목표는 예고 시작 때 고정합니다. 사슬 낙하 간격은 3.4, 기둥 반폭 0.65이며 낙하 사이에도 안전한 공간을 남깁니다. 구르기 무적과 피격 무적을 적용하고 기둥 하나가 여러 번 피해를 주지 않습니다.
- 죽은 보스의 공격/구속/사슬을 정리합니다. 보스 생존 중에는 점프·구르기·↑로 출구를 통과할 수 없습니다.

## 스토리와 보상

모르가는 양어머니를 붙잡은 왕실 사슬의 성채 닻을 지킵니다. 주인공이 어머니에게 던진 별실을 왕이 악용한다는 사실을 밝히며, 처치 후 왕도 대성당을 다음 목적지로 예고합니다. 전체 대사는 `STORY_DIALOGUE.md` 하단에 기록합니다.

처치 시 사슬 중간 고리가 떨어져 사라지고 끊어진 끝이 남습니다. 카메라는 중앙 닻을 비춘 뒤 돌아옵니다. 기존 붉은 입자 소멸, 2초 뒤 포탈, 2.5초 뒤 보물상자, 3.2초 뒤 조작 복귀를 연결했습니다. 상자는 즉시 HP 40 회복과 미보유 장비 1종을 지급합니다. 이미 장비를 모두 보유했다면 기존 중복 보상 규칙을 따릅니다. 처치 골드 250, 경험치 160은 한 번만 지급됩니다.

1스테이지에서 얻은 잿불 창 또는 홍염 고리는 계속 사용할 수 있으며 재추첨하지 않습니다. 2보스 처치 시 별도 사슬 스킬(포박 사슬 / 사슬 강림) 중 하나를 무작위 획득합니다. 조작과 효과는 CHAIN_SKILLS.md를 참고하세요. 왕도 포탈 앞 ↑로 3스테이지 왕도 대성당에 진입합니다. 모든 획득 스킬과 현재 모험 상태를 유지합니다. 상세: CHAPTER_THREE.md.

## 도트와 생성 기록

- 모드: 내장 `image_gen`, `transparent_background=true`. 기존 `Resources/Demons/Heavy.png`를 악마 계열과 화풍 참고로 사용한 신규 생성. 추가 본인 사진은 필요하지 않았습니다.
- 원본 출력: `C:/Users/sakak/.codex/generated_images/01a17569-bfbf-7233-a71a-72eece5bde92/exec-00672e4f-fb64-4a37-9928-ccca55f3f17c.png`
- 적용 파일: `Assets/Sunfall/Resources/Demons/Stage2ChainWardenV1.png` (1774×887, 알파 유지).
- 윗줄 대기 2·걷기 2, 아랫줄 내려치기 준비·타격·구속 시전·낙하 시전. 모든 포즈 배율 0.36, 프레임별 발 기준점과 넉넉한 무기 영역, 기존 포즈 전환 보간 사용. Point/Clamp/무압축/밉맵 없음. 불러오는 동안 기존 중갑 악마를 대체 표시합니다.
- 사슬 공격 예고·고리·왕실 닻은 게임 코드로 그리며 추가 이미지 편집은 하지 않았습니다.

생성 프롬프트:

```text
Use case: stylized-concept. Asset: transparent pixel-art sprite sheet for a 2D side-scrolling dark gothic action game. Reference image is style and demon-family reference only, not an edit target. Create a NEW boss: Morga, a royal-chain jailer, adult armored horned hooded female demon, imposing broad heavy black iron armour with aged brass royal crown crest, red cloth tabard, folded small bat wings, chains wound around forearms and shoulders, holds a heavy chained flail/shackle. Strong readable silhouette, disciplined game pixel art, no text or numerals, no background, no floor shadow. EXACT grid 4 columns by 2 rows, eight equal cells, all figures fully inside their cells with generous transparent padding. Every frame has same anatomical head and torso size, same heel baseline relative to cell, same pelvis X; no zoom or resizing; facing RIGHT. Top row frames: idle ready, subtle idle breath, walking left foot forward, walking right foot forward. Bottom row frames: chain weapon raised overhead windup, overhead chain strike follow-through crouch, hand extended forward casting binding magic, both hands raised invoking a royal chain storm. No long magic beams, no sweeping trails crossing cells; chain attack effects are rendered separately in engine. Pixelated edges, bronze and charcoal with muted crimson details consistent with reference. Render a landscape 4:2 grid sheet with visible genuinely transparent gutters, no labels. Adult fully armored warrior.
```

## 확인 범위

Core / Runtime / Editor / Tests / 독립 CoreChecks 컴파일만 확인합니다. 로그: `Temp/ChainWardenCompile/compile.log`. 보스 진입·출구 잠금·보상 스킬 유지·기둥 예고·점프 회피·구속 해제·2페이즈 취소·처치 보상 중복 방지에 대한 테스트 코드는 추가했지만 실행하지 않았습니다. Unity Play, 테스트 실행, 게임 QA, 플레이어 빌드는 수행하지 않았습니다.
