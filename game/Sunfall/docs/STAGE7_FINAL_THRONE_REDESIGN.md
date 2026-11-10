# 7스테이지 — 꺼진 태양의 왕좌

2026-11-04. 본인 제공 가시 왕좌실 / 검은 후드·붉은 대검 참고를 기반으로 제작.

## 적용 내용

- 전투방 3개: 빛을 잃은 왕궁 진입로, 왕의 장례 회랑, 검은 태양의 문.
- 보스방: 높은 창·찢어진 깃발·가시 장식 왕궁. 왕좌, 태양, 어머니 봉인을 배경에서 분리하여 애니메이션 처리.
- 솔베인: 검은 후드, 은빛 가시 왕관, 검은 갑옷, 붉은 균열이 있는 대검. 몸 8자세는 모든 상태에서 0.46 배율 고정, 발 기준점 수동 정렬. 공격 이펙트를 몸 이미지에서 분리.
- 등장: 앉은 자세 0–0.85초 → 기립 준비 0.85–1.55초 → 기립. 등장 대사 2.5초 이후. 실제 보스 좌표/충돌 판정 변경 없음.
- 2페이즈: 기존 체력 50% 조건 유지, 갑옷 가슴 위치를 따라가는 금빛 균열 오버레이와 안내 문구.
- 공격 외형: 왕의 대검 베기, 왕검 낙하, 회전하는 태양의 장막. 기존 데미지·예고 시간·범위·안전 구역 유지. 장막 이펙트는 위험 구역 안으로 clipping.
- 처치: 왕관 조각 0–1.2초, 어머니 봉인 해제/하강 0.65–1.55초, 태양 복원·하늘 봉합 1.15–2.5초, 새벽 배경 전환 1.5–2.8초. 봉합 자국과 구출된 어머니는 방에 남음.
- 최종 처치 대사 시작을 2.95초로 이동하여 위 연출을 먼저 보여줌. 다른 보스의 처치 대사 시점은 유지.
- 6종 대화 초상화와 가시 왕좌·붉은 대검을 반영한 대사.
- 기존 스킬/보상/장비/성장/저장 데이터 및 엔딩 진행 로직 변경 없음. Core 파일은 수정하지 않음.

## 최종 이미지 파일

아래 경로는 모두 `Sunfall/Assets/Sunfall/Resources/` 기준. 내장 image_gen으로 제작했으며 생성된 PNG를 원본 크기·알파 그대로 복사. 이미지 스크립트 편집은 하지 않음.

| 파일 | 크기 / 용도 |
|---|---|
| World/Stage7PalaceApproachHDV2.png | 2172×724 왕궁 진입로 |
| World/Stage7FuneralGalleryHDV2.png | 2172×724 장례 회랑 |
| World/Stage7SunGateHDV2.png | 2172×724 봉인 문 |
| World/Stage7ThroneHallHDV2.png | 2172×724 왕좌실 밤 |
| World/Stage7ThroneDawnHDV2.png | 2172×724 왕좌실 새벽 |
| Demons/Stage7SolveinThornV2.png | 1774×887, 4×2 몸 자세 |
| Story/BossPortraits/SolveinThornExpressionsV2.png | 1536×1024, 3×2 대화 초상화 |
| World/Stage7ThroneEndingV2.png | 1536×1024, 3×2 왕좌/검은 태양/복원된 태양/봉인된 어머니/구출된 어머니/왕관 조각 |
| Effects/Stage7RoyalSunV2.png | 1254×1254, 2×2 왕검/베기/갑옷 균열/태양 장막 |

배경은 무압축·밉맵 없음·NPOT 크기 유지·최대4096. 도트와 이펙트는 Point 필터. 기존 고딕 석재 지형 아트를 공유한다.

생성 프롬프트 전체와 여백 수정 프롬프트: [STAGE7_IMAGE_PROMPTS.md](STAGE7_IMAGE_PROMPTS.md).

## 코드

- Runtime/SunfallGame.FinalThrone.cs: 7스테이지 배경, 왕좌, 태양·봉인·구출·봉합·새벽, 등장, 갑옷 균열과 공격 이펙트.
- Runtime/SunfallGame.LateBosses.cs: 몸 자세/앵커 및 새 공격 이펙트 분기.
- Runtime/SunfallGame.Map.cs / LateMaps.cs: 새 배경·지형 연결, 이전 5–7스테이지 기하학 장식 코드를 전용 렌더러로 정리.
- Runtime/SunfallGame.BossIntro.cs / Story.cs / LateStory.cs / BossPortraits.cs: 등장·대사 시점·초상화.
- Editor/DemonArtImporter.cs: 새 솔베인 에셋 복구 목록 등록.

## 확인 결과와 범위

`tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage7FinalThroneCompile` 실행.

- Sunfall.Core: exit=0
- Sunfall.Runtime: exit=0
- Sunfall.Editor: exit=0
- Sunfall.Tests: exit=0 (소스 컴파일만 수행, 테스트 미실행)

로그: `Sunfall/Temp/Stage7FinalThroneCompile/compile.log`.
5개 배경 해상도/임포트 설정, 몸 8셀과 초상화 6셀·VFX 4셀의 투명 여백, 9개 에셋의 고유 GUID와 Resources 연결 경로를 정적으로 확인.

Unity Play와 실제 게임 QA는 실행하지 않았다. F7은 기존 개발자 7스테이지 첫 방 이동 키. 실제 화면 가독성, 왕좌와 앉은 자세의 정렬, 공격·처치 타이밍은 본인 QA 대상.
