# 보스 공격 이펙트 교체 — 2026-11-04

사용자 선택: 외형·애니메이션만 교체. Core의 공격 순서, 판정 범위, 피해량, 시전 시간과 보상은 변경하지 않음.

## 적용
- 모르가: 마름모 사슬 대신 별도 철제 고리·갈고리. 예고 중 낙하, 발동 시 타격 파편, 이후 반동과 사라짐. 화면 밖 보스의 구속 예고도 표시.
- 아우렐: 태양 베기 지면 화염, 광창 8프레임 기둥, 흑일의 위험 영역에 금빛 화염 장막.
- 4층: 심장 기둥과 파쇄 화염, 톱니 이미지 탄.
- 5층: 움직이는 거울 파편 기둥과 탄, 도약의 거울 문양.
- 6층: 푸른 장막, 초승달 탄·돌진 효과.
- 7층: 금빛 기둥·장막과 지면 파동, 왕실 광창 이미지 탄.
- 1층: 붉은 화염 탄, 돌진 잔불, 지면 파동의 사각형을 이미지 애니메이션으로 교체.
- 피격 범위를 알려주는 얇은 경계선·예고 음영·안전지대 표시는 유지.

## 구현
Runtime/SunfallGame.BossEffects.cs 공통 렌더러.
Resources/Effects/BossPillarLoopV1.png: 1536×1024, 4×2 프레임. 바닥 기준 y484/485를 맞춤.
Resources/Effects/BossChainRitualV1.png: 1254×1254, 고리/갈고리/문양/충돌 2×2 시트.
기존 PlayerSpellsV2 및 FlameLoopV1도 보스 탄·거울·달 이펙트에 재사용.
새 이미지는 내장 image_gen으로 제작, 원본 알파 보존. 이미지 생성 CLI 미사용.
Point / Clamp / 밉맵 없음 / 압축 없음 / 원본 크기 유지.
일시정지되는 visualClock과 기존 공격 타이머만 사용. OnGUI에서 게임 상태나 난수를 변경하지 않음.
위험 장막은 GUI 클립 영역으로 안전지대 바깥에 제한.

## 확인
Core / Runtime / Editor / Tests 소스 컴파일 확인.
명령: tools/Compile-AttackArt.ps1 -OutputFolder Temp/BossEffectsCompile
로그: Temp/BossEffectsCompile/compile.log
실행·Play·테스트 실행·플레이 QA는 하지 않음.
PNG 크기/투명도/프레임 여백 확인. 생성 프레임의 좌우 절단 경계 alpha 최대 0~1/255.
사용자 QA: F1~F7에서 각 보스 예고→발동→잔여 효과, 구속 사슬, 안전지대 경계와 일시정지 확인.

## 최종 이미지 프롬프트
### pillar

Production transparent PNG sprite sheet: eight looping animation frames of a vertical gothic magical FIRE PILLAR, SUNFALL dark fantasy pixel-art action game. Square canvas1536x1536, exact4 columns x2 rows, each cell384x768 tall. ONE narrow tall flame in eachcell, same height and baseline in all8, centered x50%, base y90%, tips y10%, full silhouette width x22%..78%. Animation readingorder: continuous billowing turbulent flame, twisting tongues and rising embers with changing flame contours eachframe, NOT eight identical sprites. Ivory white hot core, neutral silver pale-grey outer tongues and warm pale-gold rim, designed for code tinting crimson/gold/violet/blue. Detailed crisp pixel clusters matching polished 2D gothic game, no smooth photographic fire. Carved runic flickers within flame, faint small ground flare atbase (no huge ring). Every cell fully contained with generous transparent gutter. True transparent empty background, no checkerboard, no text labels, no borders, no characters, no scenery, no drop shadows. Sustain-loop frames ALL full-height flames, never gradually grows from tiny. Keep stable center/base acrossframes.

### chainKit

Transparent PNG 2x2 sprite asset atlas, canvas1024x1024 four equal square cells for dark gothic pixel-art game SUNFALL, detailed sharp pixel clusters. Neutral silver iron and pale ivory magic suitable for tinting in code. Top-left: ONE horizontal oval forged iron CHAIN LINK, open transparent hole, silver edge highlights dark hammered metal, occupies center x15..85% y35..65%. Top-right: ONE heavy gothic hooked chain weight arrow pointing RIGHT, angular medieval thorned iron hook, short connector onleft, occupiescenter x15..85% y20..80%, no chain string. Bottom-left: ONE magical circular ground summoning seal seen directly from ABOVE, intricate thin white-gold geometric solar tracery and broken ring, centered radius35%cell, EMPTY transparent center, no text glyphwords. Bottom-right: ONE sharp radial magic impact burst, white core sparks dust and iron fragments flying radially, centered oncell50%, radius35%. All four isolated with at least15%transparent margin. No labels, no canvas background, no colored cards, no checkerboard, NO ambient fog filling square. True alpha0 outside objects; contrast crisp against anygamebackground. NOT a painted page or infographic.


