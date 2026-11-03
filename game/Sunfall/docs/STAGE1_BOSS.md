> 이후 비행 보스와 새 공중 성소 맵으로 변경되었습니다. 현재 동작과 검증 범위는 [AERIAL_BOSS.md](AERIAL_BOSS.md)를 참고하세요. 아래는 이전 작업 기록입니다.

# 1탄 보스 — 붉은 날개의 수호자 (임시 명칭)

2026-10-26. 사용자 제공 `C:/Users/sakak/OneDrive/바탕 화면/1탄 보스 디자인.jpg`를 기준으로 제작했습니다.

## 디자인과 적용
검은 단발과 붉은 머리 끝, 붉은 눈, 머리의 박쥐 날개 장식, 금색 장식, 흰 블라우스와 검은 코르셋, 검은 쇼츠·장갑·부츠, 붉은 안감의 코트와 큰 박쥐 날개를 도트에 반영했습니다. 기존 제단 끝 임시 도형을 교체했습니다.

- 최종 PNG: `Assets/Sunfall/Resources/Demons/Stage1Boss.png`
- 1254×1254 실제 투명 알파, 내장 imagegen 생성. CLI/API 사용 없음.
- 2×2 읽는 순서: 대기 / 이동 / 손을 들어 마법 준비 / 손을 내려 마법 발동.
- 원본 생성물의 이펙트가 다른 행에 가까워 여백 보정본을 최종 사용했습니다.
- 네 사분면 분리 경계의 알파 16 초과 픽셀은 모두 0개입니다.
- 실제 실루엣 바깥 6px 여백을 포함한 원본 좌표를 사용합니다. 뿔·날개·마법까지 포함합니다.
- 발바닥 높이와 몸통 중심의 가로 기준점을 지정했습니다. 작게 그려진 시전 자세는 개별 배율로 보정했습니다.
- Point / Clamp / Texture2D / 알파 유지 / 무압축 / 밉맵 없음. `DemonArtImporter`에 추가하여 재임포트·리소스 점검 대상에 포함했습니다.
- 피격은 붉은 색상 변화, 사망은 0.6초 페이드입니다. 별도 피격·사망 시트나 긴 걷기 루프는 없습니다.
- 리소스 로딩이 지연되면 보이는 대체 도형을 표시하며 재시도합니다.

## 전투와 화면
기존 1탄 수호자 위치 X=148, 활동 범위 137~155, 체력 220을 유지합니다. 가까이 접근하면 추적하며, 붉은 지면 파동을 사용합니다.

- 준비 0.8초. 바닥 양쪽 8단위 범위를 표시하고 “붉은 파동 · 점프!” 안내.
- 준비 완료 순간 범위 내 지면 높이의 주인공에게 24 피해. 점프나 구르기 무적으로 피할 수 있습니다.
- 발동 자세와 붉은 충격파 잔상 0.32초. 잔상은 추가 피해를 주지 않습니다. 판정은 표시된 범위에 발동 순간 1회 발생합니다.
- 공격 후 재사용 1.2초. 발동 자세 동안 방향 전환·이동을 잠깐 멈춥니다.
- 커진 보스 도트에 맞춰 검과 파동의 피격 높이를 조정했습니다. 날개 전체가 피격 판정은 아닙니다.
- 주인공이 X=130 이후에 있으면 화면 상단 아래 전용 보스 이름·HP 바를 표시합니다.
- 처치 후 “수호자 처치 · 오른쪽 출구로 →” 안내. 출구에 도달하면 1탄 완료.
- 새 페이즈, 추가 공격 종류, 대화 컷신은 이번에 추가하지 않았습니다. 명칭과 수치는 사용자 QA 후 변경할 수 있습니다.

## 확인
Unity 6000.3.27f1 내장 Roslyn + 프로젝트 Bee 참조·정의·분석기 설정으로 Core / Runtime / Editor / Tests 및 독립 CoreChecks를 컴파일했습니다. 로그: `Temp/BossCompile/compile.log`.

사용자 요청대로 Play 모드, 자동 테스트 실행, 게임플레이 QA, 플레이어 빌드는 하지 않았습니다. 도트 프레임 경계는 파일 데이터로 확인했으며, 실제 게임에서의 크기·연결감·가독성은 사용자 QA 대상입니다.

## 생성 프롬프트
### 최초 생성
```text
Use case: stylized-concept. Asset: four-pose game PIXEL ART sprite sheet for the FIRST BOSS of Sunfall, based faithfully on the attached character design reference. Preserve the recognizable adult female vampire/demon character: short black bob with subtle burgundy tips, red eyes, small red bat-wing hair ornaments and gold hair brooch, pale skin, white high-neck blouse with dark bow, structured black buckled corset, black shorts, black thigh-high boots, long black gloves and split black coat with burgundy lining, large black-edged bat wings with deep crimson membranes. Fully clothed fantasy combat outfit, confident composed expression. No axe or sword: she fights with crimson wing magic. Right-facing three-quarter SIDE VIEW suited to a side-scrolling game, head and feet visible. Crisp readable pixel clusters and dark outline, restrained black/white/crimson/gold palette with slate highlights against a dark teal background. Square canvas, exactly FOUR full-body poses in a 2x2 grid with wide transparent gutters. Keep every entire silhouette INCLUDING wings, coat tails, feet and small magical effects within its own quadrant with at least 60px empty margin at edges. Same body proportions, size and grounded heel baseline in each pose. Reading order: top-left idle standing with folded wings and arms relaxed; top-right deliberate forward step with coat flowing and slightly raised wings; bottom-left attack windup, one hand raised high, wings opening, small crimson magic crescent over hand; bottom-right release, hand thrust toward the ground in front, knees slightly bent, wings swept back and compact crimson magic crescent around lowered hand. No ground wave outside character: that will be drawn by the game. Actual transparent background, no scenery, no ground shadows, text, grid, labels, signature, watermark, extra characters or detached objects. Clearly recognizable face and same costume in all four poses.
```

### 여백 보정 (최종)
```text
Use case: precise-object-edit. Edit this four-pose 2x2 boss sprite sheet ONLY for production layout. Preserve the exact character, face, outfit, colors, pixel-art rendering, and four poses. The bottom-left magic currently reaches the top row: fix that. Uniformly reduce ALL FOUR entire pose silhouettes to 75% of their present size, re-center each in its own equal quadrant, place paired heels at a consistent local baseline. Every wing, coat, boot, hand AND magic effect must be wholly inside its own quadrant with at least 50px transparent padding on every side. Keep clear horizontal AND vertical transparent gutters. Do not add or remove equipment, do not enlarge magic. Do not crop anything. Actual transparent background, no ground, text, labels or grid.
```


