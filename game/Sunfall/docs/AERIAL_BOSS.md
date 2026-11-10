> 후속 변경으로 보스 직행이 제거되었습니다. 일반 몬스터 6마리 처치 후 성소에 입장하며, 이동 보간도 적용했습니다. 현재 진행 순서와 검증 범위는 [PROGRESSION_MOTION.md](PROGRESSION_MOTION.md)를 참고하세요.

# 공중 성소 / 비행 보스 패턴 — 2026-10-26

본인 요청: 보스 돌진·붉은 마법탄·체력 절반 이하 2페이즈 추가. 작업 중 요청에 따라 새 맵과 비행 보스로 확장했습니다. 현재 마을 출발은 새 전용 전투장으로 이어집니다.

## 새 맵
- 가로 64 단위의 공중 성소 전투장. 기존 160 단위 숲 탐험 맵 구성을 대체합니다.
- 발판 8개, 높이 1.6 / 3.2 / 4.8. 왼쪽·오른쪽 상승 경로와 중앙 다리.
- 단일 점프 높이 약 2.36보다 낮은 1.6 단차로 올라갈 수 있습니다. 아래에서 발판 통과, 위에서 착지하는 기존 방식을 유지합니다.
- 지상도 연속되어 낙사 없이 전투 높이를 바꿀 수 있습니다.
- 일반 악마는 이번 전용 맵에서 생성하지 않습니다. 악마 코드와 기존 아트 파일은 프로젝트에 남아 있습니다.
- 보스 시작 (44, 2.2), 가로 활동 범위 12~57. 오른쪽 출구 X=61.
- 주인공 추적 카메라, 미니맵, 마을 안내와 결과 문구를 새 맵에 맞췄습니다.
- 현재 맵은 고정 설계이며 재도전 시 같은 구조입니다. 시드 값은 런 식별·기존 인터페이스용으로 남아 있습니다.

## 비행
보스는 중력으로 착지하지 않고 떠 있습니다. 접근·회복 중에는 주인공 높이에 맞춰 보통 Y 약 1.1~4.8로 이동하며 약간 상하로 움직입니다. 지상에서도 충분히 낮아지므로 점프 공격·불씨 파동으로 대응할 수 있습니다. 시전 예고와 돌진 중에는 조준 경로가 바뀌지 않도록 자유 비행을 중지합니다.

별도 비행 시트의 날갯짓 2자세, 마법 준비, 수평 비행 돌진 자세를 연결했습니다. 전투 위치는 움직이는 발끝 대신 몸통 기준점에 맞췄습니다. 돌진은 목표 높이를 향한 대각선 이동도 지원합니다.

## 공격
기본 순서는 **지면 파동 → 비행 돌진 → 붉은 마법탄**입니다. 벽에 막히거나 돌진할 공간이 없으면 마법탄으로 대체합니다.

| 항목 | 1페이즈 | 2페이즈 |
|---|---|---|
| 진입 조건 | HP 220~111 | HP 110 이하 |
| 지면 파동 준비 | 0.8초 | 0.65초 |
| 비행 돌진 준비 | 0.65초 | 0.5초 |
| 돌진 속도 | 16 단위/초 | 20 단위/초 |
| 돌진 가로 최대 거리 | 7 | 10 |
| 마법탄 준비 | 0.9초 | 0.65초 |
| 마법탄 개수 | 1 | 3갈래 |
| 마법탄 속도 | 9 단위/초 | 11 단위/초 |
| 일반 공격 후 재사용 | 1.2초 | 0.85초 |
| 돌진 후 재사용 | 0.9초 | 0.65초 |

- **지면 파동**: 보스 바로 아래 바닥 기준 양쪽 8 단위, 24 피해. Y 0.85 미만의 주인공만 적중합니다. 공중에 뜬 보스의 높이에서 발동하는 공격이 아닙니다. 예고는 실제 바닥에 표시하며 발판·점프·구르기 무적으로 회피 가능합니다.
- **비행 돌진**: 예고 때 주인공 위치를 기준으로 목적지와 높이를 고정합니다. 목표를 살짝 지나가되 가로 최대 거리와 전투장 경계를 지킵니다. 목적 높이는 0.5~5.2로 제한합니다. 경로를 따라 주황색 표시를 보여 줍니다. 18 피해, 한 돌진 중 접촉 판정은 한 번만 처리하므로 구르기로 통과한 뒤 뒤늦게 다시 피해를 주지 않습니다. 빠른 이동은 선분 충돌 검사로 처리합니다.
- **붉은 마법탄**: 예고 때 조준점 고정, 발사 후 추적하지 않음. 탄당 12 피해. 2페이즈에서 중심과 ±약 12도 방향의 3갈래 발사. 수명 3초, 최대 맵 경계·고도 범위를 벗어나면 소멸. 기존 한 방향 발판처럼 마법탄도 지형에 막히지 않습니다. 색이 다른 점선 예고로 진행 방향을 표시합니다. 각 탄은 충돌하면 소멸하며 주인공의 피격/구르기 무적을 적용합니다.

## 2페이즈
HP 절반 이하가 되는 첫 시뮬레이션 스텝에 한 번만 전환합니다. 준비 중인 공격과 진행 중인 돌진, 보스가 발사한 남은 탄을 취소합니다. 1초간 붉은 각성 표시 후 새 예고를 동반한 돌진부터 시작합니다. 전환 중에는 공격하지 않으며 보스가 무적이 되는 것은 아닙니다. 체력바에 현재 페이즈와 절반 눈금을 표시합니다.

보스가 죽으면 돌진·예고·마법탄을 제거하고 귀환문을 엽니다. 주인공 사망과 승리 때도 보스 탄을 정리합니다. 출구에 실제 도달해야 완료됩니다.

## 아트
내장 imagegen을 사용했습니다. CLI/API 모드는 사용하지 않았습니다.
- 비행 시트: `Assets/Sunfall/Resources/Demons/Stage1BossFlight.png` (1254×1254, 투명)
- 새 성소 배경: `Assets/Sunfall/Resources/World/AerialSanctum.png` (2172×724, 불투명)
- 지형 텍스처는 기존 `World/RuinPlatform.png`를 재사용합니다.

비행 시트는 실제 실루엣 경계로 분리했습니다. 분할선 x=660, y=670에서 알파 16 초과 픽셀 0개를 파일 데이터로 확인했습니다. 6px 여백을 둔 UV 사각형과 몸통 기준 피벗을 사용하며 원본 PNG를 복사해 알파를 보존했습니다.

## 확인 범위
Unity 6000.3.27f1 내장 Roslyn + 기존 Bee 참조/정의/분석기를 사용해 Core / Runtime / Editor / Tests / 독립 CoreChecks를 컴파일했습니다.
로그: `Temp/AerialBossCompile/compile.log`.

페이즈 전환 취소, 돌진 1회 피해, 구르기, 3갈래 탄 방향 고정·처치 시 정리, 비행 높이에 대한 테스트 소스를 추가·수정했습니다. **본인 요청대로 테스트 실행, Play 모드, 게임 QA, 플레이어 빌드는 하지 않았습니다.** 기존 중간 컴파일 로그는 최종 비행 맵 결과가 아니므로 위 로그를 기준으로 합니다. 모든 전투 수치는 본인 QA 후 조정할 초안입니다.

## 생성 프롬프트
### 비행 보스
```text
Use case: stylized-concept. Asset: FLIGHT animation sprite sheet for the same adult female red-winged first boss in attached references. Image 1 defines character identity and outfit. Image 2 defines existing game pixel-art style. Preserve short black bob with burgundy tips, red eyes, little bat-wing hair ornaments, gold brooches, white blouse, black buckled corset, shorts, long gloves, thigh-high boots and black coat with burgundy lining. FOUR full-body AIRBORNE poses in a very spacious 2x2 square transparent sheet. Reading order: top-left hovering with knees gently bent and large red bat wings lifted upwards; top-right hovering same body and pelvis position, wings flapped down/back; bottom-left airborne magic charge with one hand extended and a SMALL crimson magic orb at palm, wings spread; bottom-right horizontal forward flying dash facing right, torso leaning forward, legs trailing, wings swept back. All characters face RIGHT in side three-quarter view. Crisp pixel clusters, dark outlines, black/white/crimson/gold palette matching reference. All silhouettes must be REDUCED enough to fit comfortably inside their own quadrant with 80px empty margin, absolutely no overlap, no edge contact, no clipping of wings, coat, boots or orb. Consistent adult proportions and body size. Full transparent background and clean invisible gutters, no floor, ground shadow, flight trails, scene, text, label, grid or watermark.
```
### 새 전투장 배경
```text
Use case: stylized-concept. Asset: new wide 3:1 pixel-art background for an aerial vampire boss arena in Sunfall, a world with no sun. An open-roof ancient ruined sanctuary high above a deep blue mist-filled chasm, tall distant broken stone pillars and pointed arches on left and right framing broad open air in the middle, dark violet night clouds, a few crimson magical embers, teal moonless ambient mist. Palette cool blue slate and desaturated turquoise, muted burgundy accents. The ruins are distant silhouettes, NOT foreground platforms; keep central gameplay area clear with calm midtone mist so a black-and-red winged boss is readable. Crisp coherent pixel clusters, detailed layered pixel-art environment, original fantasy scene. Wide panoramic 3:1 opaque canvas. No characters, monsters, UI, text, sun, moon, floor collision ledges, foreground floating platforms, logos or watermark. The actual arena terrain will be overlaid separately by the game.
```


