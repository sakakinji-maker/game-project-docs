# 시작 화면 개편 · 2026-11-02

## 구성

본인이 준 Skul 시작 화면은 큰 중앙 로고와 전면 배경 구성을 참고했습니다.
두 번째 본인 이미지(사슬에 묶인 태양과 왕도 대성당)를 시작 배경으로 그대로 복사했습니다.
기존 마을 배경·오른쪽 캐릭터 액자·왼쪽 박스 메뉴 대신 중앙 로고와 간결한 텍스트 메뉴를 배치했습니다.

- SUNFALL: 금빛/상아색 고딕 로고와 가는 별실 장식.
- 한국어 제목: 별을 꿰매는 마녀.
- 이야기 시작 / 마을로 가기, 환경 설정 메뉴.
- 고정 배경, 가장자리의 느린 금빛 입자.
- 1280×720 논리 캔버스 및 기존 safe-area/레터박스 좌표 유지.
- 위/아래 방향키, Enter/Space, 마우스, 터치 입력 유지.
- 서장 시청 기록과 F10 개발자 초기화 동작 유지.

## 이미지

배경: Assets/Sunfall/Resources/Title/ChainedSunBackdropV1.png
원본: C:/Users/sakak/OneDrive/바탕 화면/KakaoTalk_261102_144334.png
원본을 수정하지 않고 복사했으며, 1672×941 RGB입니다.

로고: Assets/Sunfall/Resources/Title/SunfallWordmarkV1.png
생성 결과: 2172×724 RGBA, 투명 알파 확인.
화면에 비율을 유지하여 표시하며 최대 2048 텍스처로 임포트합니다.
도구: 내장 image_gen.imagegen, transparent_background=true.
참조 이미지는 배경의 색감/분위기 용도로만 전달했습니다.

## 컴파일

tools/Compile-AttackArt.ps1 -OutputFolder Temp/TitleRedesignCompile
Core·Runtime·Editor·Tests 모두 exit=0.
로그: Temp/TitleRedesignCompile/compile.log.
플레이·QA·테스트 실행은 하지 않았습니다.

## 로고 생성 프롬프트

```text
Use case: logo-brand. Create a premium dark gothic fantasy game title WORDMARK as an isolated transparent PNG, 1536x512 landscape 3:1. Exact text, one word only: SUNFALL. Spell S U N F A L L correctly, seven letters. Large elegant custom sharp gothic serif capitals, harmonious kerning, instantly readable, sophisticated antique ivory and pale gold metal with fine engraved details, slight warm glow limited to letter edges. A thin golden thread subtly stitches across the lower serifs, with restrained needle-like flourishes extending horizontally at each side. Dominant text fills 85% canvas width and 60% canvas height centered, fully contained including all flourishes. No giant crest above, no extra words, no subtitle, no frame, no black rectangle, no scenery, no shadows outside the tightly bounded logo. True transparent background. The reference image is PALETTE AND MOOD reference only: a chained golden sun above a midnight gothic cathedral. Do not reproduce the scene in this asset. Deliver only the gold ivory SUNFALL wordmark with delicate golden thread decoration, clean readable at 650px wide.
```

