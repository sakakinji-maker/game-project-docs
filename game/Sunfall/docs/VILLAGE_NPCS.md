# 마을 NPC 도트 적용 — 2026-10-27

사용자가 제공한 세 이미지를 기준으로 built-in image_gen 도구로 제작한 투명 도트 시트를 적용했다.

| NPC | 반영한 특징 | 리소스 |
|---|---|---|
| 대장장이 | 짧은 회색 머리, 베이지 작업복, 밧줄 허리띠, 망치와 집게 | Assets/Sunfall/Resources/Village/BlacksmithV1.png |
| 상인 | 금발 묶음머리, 베이지 모자, 갈색 망토와 붉은 안감, 망토 안 목검, 가죽 가방 | Assets/Sunfall/Resources/Village/MerchantV1.png |
| 아린 | 갈색 웨이브 머리, 크림색 블라우스, 갈색 코르셋·긴 치마, 펜던트, 지도통과 등불 | Assets/Sunfall/Resources/Village/ArinV1.png |

각 시트는 1254×1254 RGBA, 2열×2행이다. 대기 2장과 대화/인사 2장, 총 12개의 자세가 들어 있다. 세 PNG 모두 투명 알파가 있으며 모든 실루엣과 소품이 분리된 프레임 안에 들어간 것을 원본 알파 영역으로 확인했다. 기존 주인공 도트를 스타일 참고로 사용했다.

## 적용 내용

- 마을 X=220 대장간 앞에 대장장이, X=780 가판 앞에 상인, 기존 X=490에 아린을 배치했다.
- 주인공 평상시 높이를 기준으로 상인/아린 약 70px, 대장장이 약 77px. 캐릭터마다 네 자세의 배율을 고정해 대화할 때 커지는 문제를 방지한다.
- 발바닥 기준점을 바닥 Y=540에 맞추고 주인공 방향을 바라본다. 주인공이 겹친 좁은 구간에서는 좌우 방향을 계속 바꾸지 않는다.
- 가까이(145px 이내) 가면 대화 버튼이 표시된다. 대화 중 이동·출발 입력 차단과 Esc 닫기를 기존 흐름대로 유지한다.
- 대장장이와 상인에게 소개/전투 조언 대사를 추가했다. 거래나 강화 경제 시스템은 이 작업에 포함하지 않는다.
- 아린의 기존 전투 안내는 보존했다. 아린의 보라색 도형 표시를 새 도트로 교체했다.
- NPC가 말할 때 대화창에 해당 NPC의 인사 도트를 크게 표시한다. 플레이어 답변에서는 기존 주인공 초상화를 표시한다. 별도의 새 초상화 파일은 생성하지 않았다.
- 아린의 월드 대화 자세는 등불을 같은 쪽에 든 프레임을 사용해 손 위치가 갑자기 바뀌지 않게 했다.
- 대장간/등불 쉼터/상인 가판 표지와 작은 작업 소품을 추가했다.
- 전용 VillageArtImporter가 Texture2D / Point / Clamp / 원본 알파 / 무압축 / mipmap off 설정을 유지한다. 임포트 중 리소스가 없으면 재시도한다.

코드: Runtime/SunfallGame.VillageNpcs.cs, Runtime/SunfallGame.Village.cs, Editor/VillageArtImporter.cs.

## 컴파일 확인

Unity 6000.3.27f1 Roslyn 및 현 프로젝트 Bee 참조로 Core / Runtime / Editor / Tests 어셈블리 컴파일 성공. 오류·경고 출력 없음.
로그: Temp/VillageNpcCompile/compile.log.
Unity Play, 게임 실행, 테스트 실행, 인게임 QA는 수행하지 않았다.

재컴파일: 프로젝트 상위 폴더에서 `./tools/Compile-AttackArt.ps1 -OutputFolder Temp/VillageNpcCompile`.

사용자가 언급한 Unity 메시지는 별도 확인 중이다. 확인한 Editor.log에는 C# 컴파일 오류가 없었으며, 메시지 원문을 요청했다. 컴파일 성공을 그 메시지 해결 확인으로 간주하지 않는다.

## 생성 프롬프트

built-in image_gen / transparent_background=true. 각 요청의 첫 번째 참조는 사용자 이미지, 두 번째 참조는 Hero/HeroAtlas.png. 생성 원본은 기본 generated_images 위치에 보존하고 선택한 PNG를 프로젝트로 복사했다.

### BlacksmithV1
참조: 중세 대장관.jpg

Use case: stylized-concept. Asset type: Sunfall medieval dark-fantasy village NPC pixel sprite sheet. Image1 is the identity/costume reference; Image2 is the existing game's pixel-art style and body proportions reference only (do not copy the heroine's costume or gender). Create exactly FOUR full-body sprites of the SAME NPC in a clean TWO-column TWO-row grid on a 1024x1024 transparent RGBA canvas. Each cell 512x512, character including props inside x=100..412 and y=85..440 relative to its cell. Generous empty transparent margins. Same head and body size, same grounded heel baseline y=440, hip center x=256 in all four cells; NO scale changes or body translation between frames. All face slightly RIGHT in side three-quarter view appropriate for side-scrolling game. Read left to right top to bottom: 1 relaxed idle, 2 subtly breathing idle (tiny shoulder/hair change), 3 talking with one small welcoming hand gesture, 4 talking settling back toward idle. Anatomically adult, stylized about four heads tall, crisp deliberate pixel clusters and dark outlines, restrained palette matching the game's hero, readable at 70-pixel on-screen height. No smooth vector lines, no photoreal textures, no text, numbers, watermark, panels, scenery, checkerboard or floor shadow. Complete head, hands and boots in EVERY frame. True alpha background, never matte. Character specification: Middle-aged male village BLACKSMITH, sturdy stocky physique, short gray cropped hair, weathered clean-shaven face and calm stern expression. Reference beige rough sleeveless work apron/tabard over dark charcoal long sleeves, rope waist belt and small hanging pouch, dark practical trousers and worn leather boots. Carry small iron smithing hammer in one hand held low, tongs in other hand. Mild soot marks, warm amber metal accents. No large forge or anvil in the sheet. The third pose raises the tongs a little while talking, not a large attack swing. Keep a grounded, hardworking craftsman silhouette.

### MerchantV1
참조: 중세 상인.jpg

Use case: stylized-concept. Asset type: Sunfall medieval dark-fantasy village NPC pixel sprite sheet. Image1 is the identity/costume reference; Image2 is the existing game's pixel-art style and body proportions reference only (do not copy the heroine's costume or gender). Create exactly FOUR full-body sprites of the SAME NPC in a clean TWO-column TWO-row grid on a 1024x1024 transparent RGBA canvas. Each cell 512x512, character including props inside x=100..412 and y=85..440 relative to its cell. Generous empty transparent margins. Same head and body size, same grounded heel baseline y=440, hip center x=256 in all four cells; NO scale changes or body translation between frames. All face slightly RIGHT in side three-quarter view appropriate for side-scrolling game. Read left to right top to bottom: 1 relaxed idle, 2 subtly breathing idle (tiny shoulder/hair change), 3 talking with one small welcoming hand gesture, 4 talking settling back toward idle. Anatomically adult, stylized about four heads tall, crisp deliberate pixel clusters and dark outlines, restrained palette matching the game's hero, readable at 70-pixel on-screen height. No smooth vector lines, no photoreal textures, no text, numbers, watermark, panels, scenery, checkerboard or floor shadow. Complete head, hands and boots in EVERY frame. True alpha background, never matte. Character specification: Adult male travelling MERCHANT with friendly slightly roguish smile, sandy blond hair tied in a low ponytail, soft tan medieval cap tilted to one side. Dark brown open travelling cloak with rust-red lining, white rolled-sleeve linen shirt, reddish vest, brown belt, dark trousers, practical reddish-brown boots, cross-body leather satchel. A few small wooden training swords and wrapped wares neatly hanging INSIDE the cloak, inspired by the reference, NOT floating around him. Idle cloak partly open; talking gesture opens it only slightly to show wares, keeping full silhouette within cell. Warm ochre and earthy browns.

### ArinV1
참조: 중세 캐릭터.jpg

Use case: stylized-concept. Asset type: Sunfall medieval dark-fantasy village NPC pixel sprite sheet. Image1 is the identity/costume reference; Image2 is the existing game's pixel-art style and body proportions reference only (do not copy the heroine's costume or gender). Create exactly FOUR full-body sprites of the SAME NPC in a clean TWO-column TWO-row grid on a 1024x1024 transparent RGBA canvas. Each cell 512x512, character including props inside x=100..412 and y=85..440 relative to its cell. Generous empty transparent margins. Same head and body size, same grounded heel baseline y=440, hip center x=256 in all four cells; NO scale changes or body translation between frames. All face slightly RIGHT in side three-quarter view appropriate for side-scrolling game. Read left to right top to bottom: 1 relaxed idle, 2 subtly breathing idle (tiny shoulder/hair change), 3 talking with one small welcoming hand gesture, 4 talking settling back toward idle. Anatomically adult, stylized about four heads tall, crisp deliberate pixel clusters and dark outlines, restrained palette matching the game's hero, readable at 70-pixel on-screen height. No smooth vector lines, no photoreal textures, no text, numbers, watermark, panels, scenery, checkerboard or floor shadow. Complete head, hands and boots in EVERY frame. True alpha background, never matte. Character specification: ARIN, adult female village guide and keeper of the lantern. Preserve reference shoulder-length wavy chestnut brown hair with center part, warm hazel-gold eyes, composed friendly face. Cream off-shoulder linen blouse with puff sleeves, fitted dark brown laced corset, long split brown skirt over practical dark leggings and brown boots, small bronze pendant, belt pouch and rolled map case. Outfit remains practical and tasteful, no exaggerated cleavage. Hold a SMALL warm amber lantern low at her left side, other hand gives a modest welcoming gesture. Warm earth palette and a modest gold lantern glow confined close to lantern. No weapon or crown.
