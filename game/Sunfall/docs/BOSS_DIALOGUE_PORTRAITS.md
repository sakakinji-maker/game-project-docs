# 보스 대화 초상화 · 2026-11-02

1~7층 보스의 기존 도트 디자인을 참고해 정면 상반신 초상화와 표정 6종씩 총 42종을 생성했습니다. 내장 image_gen 도구를 사용했고, 생성 원본은 .codex/generated_images에 보존했습니다.

## 리소스

모두 Assets/Sunfall/Resources/Story/BossPortraits 아래에 있습니다.
- 1층: RedWingExpressionsV1.png
- 2층: MorgaExpressionsV1.png
- 3층: AurelExpressionsV1.png
- 4층: VarkanExpressionsV1.png
- 5층: LysseaExpressionsV1.png
- 6층: NoctisExpressionsV1.png
- 7층: SolveinExpressionsV1.png

각 파일은 1536×1024 RGBA, 3열×2행입니다. 왼쪽 위부터 기본·비웃음·분노·놀람·고통·체념 순서이며 얼굴을 정사각형으로 표시해 세로로 늘어나지 않게 했습니다. 갑옷/가면 보스는 눈과 고개 자세로 감정을 표현합니다.

## 대사 연결

- 등장 및 설명: 기본 또는 비웃음.
- 위협과 전투 선언: 분노.
- 봉인·왕관이 무너지는 순간: 놀람.
- 붉은 날개 수호자/바르칸의 처치 대사와 아우렐의 마지막 경고: 고통.
- 모르가의 마지막 경고와 녹티스 처치: 체념.
- 대사에 StoryExpression을 지정하며, 지정하지 않은 대사는 기본 표정입니다. 생성된 여섯 표정을 모두 억지로 재생하지 않고 현재 대사의 의미에 맞춰 사용합니다.
- 플레이어/NPC 초상화와 스토리 시청 기록·저장 키는 그대로 유지합니다. 이미 본 대사는 기존 규칙대로 건너뜁니다.
- 초상화 임포트가 끝나지 않았을 때는 기존 보스 도트를 표시하고 다시 로드합니다.

## 검증

일곱 이미지의 해상도, RGBA 및 알파 채널과 Resources 경로를 확인했습니다.
tools/Compile-AttackArt.ps1 -OutputFolder Temp/BossPortraitCompile 실행 결과 Core·Runtime·Editor·Tests 모두 exit=0.
로그: Temp/BossPortraitCompile/compile.log.
Unity Play, QA, 테스트 실행은 하지 않았습니다.

## 생성 프롬프트

도구: 내장 image_gen.imagegen. transparent_background=true. 각 보스의 기존 이미지 하나를 referenced_image_paths로 전달했습니다.

### 1층 RedWing

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage1Boss.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Stage 1 red-wing guardian: adult female demon, short black bob with burgundy ends, red eyes, pointed ears, small red batwing head ornaments, asymmetric golden filigree hair ornament, black/gold corset over white ruffled blouse and red gemstone at collar, red bat wings folded behind shoulders.
```

### 2층 Morga

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage2ChainWardenV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Morga, royal chain warden: female demon in heavy charcoal plate with gold edges, hooded crown helmet with curled ram horns, shadowed pale lower face and ominous eyes visible within the hood, chain details and dark red cloth, red batwing roots. Keep her hood/helmet on; express through visible eyes and mouth.
```

### 3층 Aurel

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage3SolarRegentV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Aurel, solar seal regent: crowned demon knight in black plate and elaborate gold trim, circular sun halo behind the helmet, red eyes in dark visor opening, ivory and burgundy cloth, red wing roots. Keep crown and helmet; expressions through visible supernatural eyes, visor shadows and posture, no unrelated human face.
```

### 4층 Varkan

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage4VarkanV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Varkan, heart keeper: massive masculine armored demon, black riveted plate with brass gear shoulders, horned closed iron visor, red mechanical heart behind gold chest cage, dark crimson cape. Keep closed helmet and gear silhouette; emotions through red eye slit shapes/light and subtle head angle, no human face, no removed mask.
```

### 5층 Lyssea

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage5LysseaV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Lyssea, mirror sovereign: elegant adult female demon with pale porcelain face, long lavender hair, black branching swept horns, violet eyes, silver and violet armor with gold edges, amethyst mirror shards behind shoulders. Preserve pointed ears and pale face, depict visible expressive eyes and mouth.
```

### 6층 Noctis

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage6NoctisV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Noctis, eclipse wing: androgynous adult demon, long midnight blue hair, pale face, cold blue eyes, silver crescent crown, black-silver armor with blue jewel and deep navy cloak, black batwing roots. Preserve identity; show visible face and restrained ominous expressions.
```

### 7층 Solvein

참조: C:/Users/sakak/Downloads/Sunfall_Unity_Prototype/Sunfall/Assets/Sunfall/Resources/Demons/Stage7SolveinV1.png

```text
Use case: identity-preserve. Reference image is the existing game character sprite sheet: use ONLY its identity, helmet/hair, costume, palette and motifs, not its full-body pose/layout. Create a polished dark gothic illustrated DIALOGUE PORTRAIT EXPRESSION ATLAS for SUNFALL. EXACTLY SIX front-facing chest-up portraits of the SAME character, in a strict 3 columns x 2 rows equal-cell grid on a landscape 1536x1024 RGBA canvas. Every cell is 512x512. Each portrait fully contained in its cell with 30px transparent outer margin. Same face size, eye line, chest crop and centered head throughout; direct frontal camera (not profile). Heads, crowns, horns and shoulders must not cross cell borders. Bust height about 440px, face at same height in each cell. Rich clean linework, controlled painterly cel shading, readable facial features at 215px dialogue size. No frame, no UI, no text, no letters, no numbers, no props crossing cells, no scene background. True transparent background. Reading order left to right top to bottom: 1 composed stern neutral; 2 arrogant mocking smirk/contempt; 3 furious challenge; 4 startled disbelief; 5 strained pain after defeat (no blood); 6 exhausted resignation/fading resolve. Genuinely distinguish each emotion, keep character design consistent. For masked characters retain the mask and convey emotion through visible eyes and restrained posture, not a smile painted on metal. Character: Solvein, fallen sun king: imposing masculine demon king in full black-gold armor, tall broken golden crown and curved horns, dark closed faceguard with golden burning eye slits, black eclipse sun in chest with golden cracks, crimson royal cape. Keep helmet; express through supernatural eyes and head attitude, no exposed new human face.
```


