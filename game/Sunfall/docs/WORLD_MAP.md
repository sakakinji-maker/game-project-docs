> 이후 비행 보스와 새 공중 성소 맵으로 변경되었습니다. 현재 동작과 검증 범위는 [AERIAL_BOSS.md](AERIAL_BOSS.md)를 참고하세요. 아래는 이전 작업 기록입니다.

# 넓은 숲·유적 맵 / 증강 제거 (2026-10-26)

## 현재 흐름
마을 → 잊힌 숲 → 무너진 회랑 → 빛이 꺼진 제단 → 수호자 처치 → 오른쪽 출구 통과.
세 구역은 로딩이나 보상 화면 없이 하나의 연속 맵으로 이어집니다. 일반 악마를 전부 처치할 필요는 없으며 수호자 처치가 출구 해제 조건입니다. 출구에 실제로 도달해야 승리합니다.

## 제거
증강 선택 화면, Gift 종류, Offers 후보, Build 누적 목록, Choose 처리, Reward 상태, 방마다 발생하던 능력치 상승과 방 전환을 제거했습니다. HUD/결과 화면의 빌드·보상 표시, 아린의 불씨 선택 안내도 수정했습니다. 불씨 파동은 기존 전투 스킬로 유지합니다.

## 맵
- 가로 32 → 160 월드 단위, 기존 전투장 폭의 5배. 화면 배율 34px/단위 유지.
- 연속 지상 경로 + 발판 24개. 높이 1.7 / 3.4 / 5.1 / 6.8.
- 4개 상단 경로의 상승 계단은 각 단차 1.7이고 인접 발판이 겹칩니다. 기존 단일 점프의 이론상 최대 높이 약 2.36 이내입니다. 최상단에서 내려오는 쪽은 크게 낙하할 수 있습니다.
- 낙사 구덩이 없이 지상으로도 맵 끝까지 이동할 수 있습니다. 발판은 아래에서 통과하고 위에서 착지하는 방식입니다.
- 주인공을 부드럽게 따라가는 가로·세로 카메라, 배경의 느린 스크롤, 화면 밖 적/지형 그리기 생략.
- 미니맵: 지형, 흰색 주인공, 적, 주황색 보스, 봉인/해제된 출구.
- 근접병 4 / 궁수 4 / 중갑병 4 / 테스트 수호자 1. 궁수는 높이 3.4의 발판 위에 배치.
- 적은 자기 활동 범위 안에서 움직이며 멀리 있는 적이 맵 전체를 건너 몰려오지 않습니다. 궁수는 발판 끝에서 멈춥니다.
- 공격·검·파동·화살 판정과 표시를 적의 높이, 새 맵 경계에 맞췄습니다.
- 별도 증강이나 회복 보상 없이 HP 100, 기본 공격력 24로 시작합니다. 난이도는 본인 QA 뒤 조정할 임시값입니다.
- 후속 보스 아트 변경으로 제단 끝 수호자를 본인 참고 디자인의 도트로 교체했습니다. 기존 지면 공격에 준비·발동 모션과 전용 체력바를 연결했습니다. `STAGE1_BOSS.md` 참고.

## 아트
제공된 `C:/Users/sakak/OneDrive/바탕 화면/예시맵.jpg`의 숲·폐허·다층 발판 구성을 참고했습니다. 햇빛이 없는 세계에 맞춰 청록빛 안개, 푸른 석재, 부서진 아치, 작은 황금빛 등불로 재해석했습니다.

내장 imagegen으로 생성한 원본 PNG를 프로젝트에 복사했습니다. CLI/API 모드는 사용하지 않았습니다.
- `Assets/Sunfall/Resources/World/SunlessForest.png`: 2172×724 불투명 파노라마 배경
- `Assets/Sunfall/Resources/World/RuinPlatform.png`: 2172×724 투명 지형 스트립

배경은 장식이며 실제 발판 충돌은 Core의 Platform 좌표가 결정합니다. 지형 표면에는 착지 높이를 읽기 위한 얇은 청록색 선을 표시합니다. Point / Texture2D / 무압축 / 밉맵 없음 / NPOT 크기 유지 / 최대 4096 임포트 설정입니다.

## 확인 범위
Unity 6000.3.27f1 내장 Roslyn 및 프로젝트 Bee 참조·정의·분석기 설정으로 Core / Runtime / Editor / Tests를 컴파일했습니다. 독립 CoreTests 프로그램도 컴파일만 했습니다. 결과: `Temp/MapCompile/compile.log`.

기존 보상 테스트를 탐험 지속, 출구 해제 조건, 능력치 유지, 고지대 적 배치 검증으로 갱신했습니다. 본인 요청대로 테스트 실행, Play 모드, 게임플레이 QA, 플레이어 빌드는 하지 않았습니다. 과거 버전 테스트 통과 기록은 이번 변경의 결과가 아닙니다.

## 생성 프롬프트
### 배경
```text
Use case: stylized-concept. Asset: wide panoramic pixel-art BACKGROUND ONLY for a 2D side-scrolling action game called Sunfall, a world whose sun has vanished. Attached image is a composition/medium reference for layered forest and ruined stone arches, NOT an edit target. Create an original nightfall forest of tall ancient teal-grey trees, moss-covered broken archways, ruined distant aqueduct, drifting pale cyan mist, tiny sparse amber embers. Palette blue charcoal, desaturated turquoise, cool moss, muted violet shadows, occasional warm lantern glints. Atmospheric readable mid-dark values, never pitch-black, dark silhouettes can stand out against pale mist. Layered depth, pixel clusters and crisp pixel edges, cohesive detailed pixel-art environment. Very wide 3:1 landscape panorama, with natural continuous forest at both edges, matching edge colors for scrolling. Distant scenery only: no foreground collision ledges or floor across the bottom, no characters/enemies/weapons/UI/text/logos/watermark, no visible sun or bright daytime sky. The lower and central regions stay relatively quiet behind gameplay. Full opaque background.
```

### 지형
```text
Use case: stylized-concept. Asset: isolated terrain platform strip for Sunfall side-view 2D pixel-art game. Draw ONE long horizontal rectangular chunk of ancient dark slate stone blocks and fractured bedrock, moss and short cool-teal grass covering its FLAT LEVEL top, small roots hanging below. Palette dark blue slate, muted teal moss, pale cyan edge highlights, tiny amber flecks. Strong readable top collision edge, never sloped. Crisp pixel clusters and hard outlines, side-on orthographic view, no perspective. Wide 3:1 transparent canvas, the single platform fills most of canvas width with safe empty margins. Entire chunk visible. Platform thickness about one quarter of its width. Top grass at same level throughout; no tall decorative plants, no trees, no pillars, no characters, no text, no background, no ground shadow, no floating separate rocks. Actual transparent alpha around this one isolated platform. Suitable to stretch slightly and repeat across floor and raised walkways. Match a moody sunless ruined forest.
```



