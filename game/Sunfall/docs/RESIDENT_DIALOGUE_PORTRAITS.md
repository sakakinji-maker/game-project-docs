# 주인공·마을 NPC 대화 표정

주인공, 대장장이, 떠돌이 상인, 아린 각 6종, 총 24종의 정면 상반신 초상화입니다.
기존 캐릭터 디자인을 참조하여 내장 image_gen.imagegen 도구로 생성했습니다.

## 리소스

디렉터리: Assets/Sunfall/Resources/Story/ResidentPortraits/
- HeroExpressionsV1.png
- BlacksmithExpressionsV1.png
- MerchantExpressionsV1.png
- ArinExpressionsV1.png

1536×1024 RGBA, 3열×2행, 셀당 512×512.
왼쪽 위부터 기본·미소·걱정·결의·놀람·슬픔 순서입니다.
보스 시트의 순서와 다르므로 ResidentExpressionCell에서 명시적으로 매핑합니다.
정사각형으로 표시해 얼굴이 세로로 늘어나지 않습니다.
보스의 기존 표정 enum 숫자 및 저장 키는 유지합니다.

## 상황별 연결

- 어머니의 행방을 묻거나 위험을 경고할 때: 걱정.
- 돌아오지 않는 어머니와 잊혀 가는 기억을 이야기할 때: 슬픔.
- 격려, 감사, 다시 만날 약속, 마지막 귀환: 미소.
- 보스에게 맞서거나 구출을 다짐할 때: 결의.
- 왕이 별실을 이용한다는 사실을 알게 될 때: 놀람.
- 평상시 질문: 기본.
- 대장간은 결의, 상점은 기본 표정으로 열립니다. 거래 성공 시 미소, 거래 실패 응답 시 걱정으로 바뀝니다.

총 50줄에 표정을 지정했습니다. 현재 대사에서 쓰지 않는 표정도 향후 사용을 위해 보존합니다.
서장은 기존 전체 화면 애니메이션을 유지합니다. 서장 대사 데이터에도 표정을 지정했지만 애니메이션에 별도 초상화 패널을 추가하지 않습니다.
양어머니·나레이터·이름 없는 주민의 표현, 인게임 도트와 움직임, 기존 스토리 시청 기록은 그대로 유지합니다.
이미지가 임포트 중이면 이전 초상화로 표시하고 재시도합니다.

## 검증

Core·Runtime·Editor·Tests 컴파일 모두 exit=0.
명령: tools/Compile-AttackArt.ps1 -OutputFolder Temp/ResidentPortraitCompile
로그: Temp/ResidentPortraitCompile/compile.log
Unity Play, QA 및 테스트 실행은 하지 않았습니다.

## 생성 프롬프트

도구: 내장 image_gen.imagegen, transparent_background=true.
참조를 referenced_image_paths로 전달했습니다.
생성 원본은 .codex/generated_images에 보존하고 프로젝트 리소스로 복사했습니다.

### Hero

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Hero/HeroPortrait.png

```text
Use case: identity-preserve. Input image is a reference for character identity and clothes only. Create a polished dark gothic fantasy illustrated dialogue portrait EXPRESSION ATLAS. Exactly SIX frontal chest-up portraits of this SAME character in a strict 3 columns by 2 rows, equal square cells on a 1536x1024 landscape RGBA canvas. True transparent background, no scenery or checkerboard. Each portrait fits inside its 512x512 cell with 24px transparent margin; identical head size, eye line, chest crop and position throughout. Direct frontal view, highly readable face at 200px. Elegant clean linework, detailed restrained painterly cel shading matching gothic visual novel portraits; not pixel art. Reading order left to right top to bottom: 1 calm attentive neutral; 2 warm gentle reassuring smile; 3 worried and concerned with furrowed brows; 4 firm determined resolve and serious eyes; 5 startled surprise, widened eyes; 6 sorrowful sadness and lowered brows, restrained not comedic. Six distinct emotions; preserve face, age, hairstyle, costume and palette. No text, labels, borders, UI, extra characters. No head or shoulders crossing cell boundaries. Character: Adult female heroine, dusty burgundy hair with straight bangs and a low messy tied bun, long sidelocks, rose eyes, pale skin, white asymmetric off-shoulder draped blouse, black long gloves and black waist belt. Preserve the reference face and clothes. No cigarette.
```

### Blacksmith

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Village/BlacksmithV1.png

```text
Use case: identity-preserve. Input image is a reference for character identity and clothes only. Create a polished dark gothic fantasy illustrated dialogue portrait EXPRESSION ATLAS. Exactly SIX frontal chest-up portraits of this SAME character in a strict 3 columns by 2 rows, equal square cells on a 1536x1024 landscape RGBA canvas. True transparent background, no scenery or checkerboard. Each portrait fits inside its 512x512 cell with 24px transparent margin; identical head size, eye line, chest crop and position throughout. Direct frontal view, highly readable face at 200px. Elegant clean linework, detailed restrained painterly cel shading matching gothic visual novel portraits; not pixel art. Reading order left to right top to bottom: 1 calm attentive neutral; 2 warm gentle reassuring smile; 3 worried and concerned with furrowed brows; 4 firm determined resolve and serious eyes; 5 startled surprise, widened eyes; 6 sorrowful sadness and lowered brows, restrained not comedic. Six distinct emotions; preserve face, age, hairstyle, costume and palette. No text, labels, borders, UI, extra characters. No head or shoulders crossing cell boundaries. Character: Mature male blacksmith, short cropped salt-and-pepper hair, weathered angular clean-shaven face, grey eyes, broad sturdy shoulders, rough beige leather apron/tabard over charcoal high-neck sleeves, small leather pendant. Plain medieval craftsman, not a knight. No weapon in shot.
```

### Merchant

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Village/MerchantV1.png

```text
Use case: identity-preserve. Input image is a reference for character identity and clothes only. Create a polished dark gothic fantasy illustrated dialogue portrait EXPRESSION ATLAS. Exactly SIX frontal chest-up portraits of this SAME character in a strict 3 columns by 2 rows, equal square cells on a 1536x1024 landscape RGBA canvas. True transparent background, no scenery or checkerboard. Each portrait fits inside its 512x512 cell with 24px transparent margin; identical head size, eye line, chest crop and position throughout. Direct frontal view, highly readable face at 200px. Elegant clean linework, detailed restrained painterly cel shading matching gothic visual novel portraits; not pixel art. Reading order left to right top to bottom: 1 calm attentive neutral; 2 warm gentle reassuring smile; 3 worried and concerned with furrowed brows; 4 firm determined resolve and serious eyes; 5 startled surprise, widened eyes; 6 sorrowful sadness and lowered brows, restrained not comedic. Six distinct emotions; preserve face, age, hairstyle, costume and palette. No text, labels, borders, UI, extra characters. No head or shoulders crossing cell boundaries. Character: Adult male travelling merchant, sandy blond hair tied low at the back, hazel eyes, clean-shaven friendly face, beige cloth beret, brown travelling cloak, rust-red vest and white shirt, leather shoulder strap. Preserve the male identity and reference clothes.
```

### Arin

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Village/ArinV1.png

```text
Use case: identity-preserve. Input image is a reference for character identity and clothes only. Create a polished dark gothic fantasy illustrated dialogue portrait EXPRESSION ATLAS. Exactly SIX frontal chest-up portraits of this SAME character in a strict 3 columns by 2 rows, equal square cells on a 1536x1024 landscape RGBA canvas. True transparent background, no scenery or checkerboard. Each portrait fits inside its 512x512 cell with 24px transparent margin; identical head size, eye line, chest crop and position throughout. Direct frontal view, highly readable face at 200px. Elegant clean linework, detailed restrained painterly cel shading matching gothic visual novel portraits; not pixel art. Reading order left to right top to bottom: 1 calm attentive neutral; 2 warm gentle reassuring smile; 3 worried and concerned with furrowed brows; 4 firm determined resolve and serious eyes; 5 startled surprise, widened eyes; 6 sorrowful sadness and lowered brows, restrained not comedic. Six distinct emotions; preserve face, age, hairstyle, costume and palette. No text, labels, borders, UI, extra characters. No head or shoulders crossing cell boundaries. Character: Adult woman Arin the lantern keeper, shoulder length wavy chestnut hair parted at center, golden hazel eyes, cream off-shoulder peasant blouse, dark brown laced corset, small bronze pendant. Warm reliable medieval villager. No new crown or armor.
```


