# 3스테이지 왕도 대성당 · 아우렐 비주얼 교체

## 적용 내용
- 대성당 진입로 / 왕실 회랑 / 태양 봉인 예배당 / 태양 봉인의 내전: 방마다 독립된 2172×724 파노라마.
- 표시 폭 2070px, 원본 비율 유지. 1스테이지와 같은 배경 픽셀 밀도. 아틀라스 행 확대 없음.
- 청록 지붕과 푸른 안개에서 짙은 석조 회랑, 금빛 태양 봉인으로 이어지는 구성.
- 이전의 불투명 사각 기둥·창문·제단을 제거. 새 대성당 석재 발판은 기존 충돌 위치에 맞춰 원본 비율로 반복.
- 아우렐: 흰 수의, 어두운 후드, 호박색 눈, 부서진 금빛 후광, 검은 유리 날개. 참고 보스의 분위기를 활용한 별도 외형.
- 인게임 8자세: 대기 2 / 이동 2 / 베기 준비·발동 / 광창 시전 / 흑일 시전. 모든 자세 배율 0.42 고정, 발 기준점 개별 지정.
- 대화 6표정: 평상시 / 냉소 / 분노 / 놀람 / 부상 / 체념. 기존 StoryExpression 연결.
- 보스방 봉인 사슬과 처치 후 금빛 바늘땀은 배경 태양 문양과 같은 시차로 이동.
- 기존 방 구조, 적 배치, 보스 공격 패턴·능력치, 스킬 보상, 대사, 저장 데이터 유지. 5~7층 석재 렌더링 유지.

## 새 리소스
경로는 Unity 프로젝트 기준.
| 용도 | 경로 | 실제 크기 |
|---|---|---|
| 외곽 | Assets/Sunfall/Resources/World/Stage3ApproachHDV2.png | 2172×724 |
| 회랑 | Assets/Sunfall/Resources/World/Stage3CloisterHDV2.png | 2172×724 |
| 예배당 | Assets/Sunfall/Resources/World/Stage3ChapelHDV2.png | 2172×724 |
| 내전 | Assets/Sunfall/Resources/World/Stage3SanctumHDV2.png | 2172×724 |
| 발판 | Assets/Sunfall/Resources/World/Stage3CathedralStoneV2.png | 2172×724 |
| 보스 | Assets/Sunfall/Resources/Demons/Stage3AurelEclipseV2.png | 1774×887 |
| 표정 | Assets/Sunfall/Resources/Story/BossPortraits/AurelEclipseExpressionsV2.png | 1536×1024 |

이전 이미지는 되돌리기 용도로 보존하며 런타임에서 새 이름을 참조한다.
월드 이미지는 최대 4096 / 보스·초상화는 최대 2048, 압축·밉맵·NPOT 리사이즈 없음.
배경과 보스는 Point, 초상화는 Bilinear.

## 확인
2026-11-03: tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage3CathedralCompile
Core / Runtime / Editor / Tests 소스 컴파일 모두 exit=0.
로그: Temp/Stage3CathedralCompile/compile.log.
이미지 해상도, 투명도, 프레임 경계, 임포트 최대 크기 확인.
게임 실행, Unity Play, 테스트 실행, 플레이 QA는 하지 않음.
사용자 확인: 개발자 F3 → 3스테이지 첫 방. 네 방의 배경, 발판 발 위치, 보스 공격 자세와 초상화, 처치 후 봉합 연출.

## 제작 방식과 최종 프롬프트
내장 image_gen 도구로 각 이미지를 개별 생성. CLI/API 대체 미사용.
사용자 외관·내부 이미지는 건축 분위기, 보스 이미지는 분위기만 참고.
SunlessForest.png는 기존 게임의 픽셀 밀도와 색감 기준.
초상화는 새로 생성한 보스 시트를 외형 기준으로 사용.
생성 원본은 Codex generated_images에 보존하고 최종본을 위 Resources 경로에 복사.

### exterior

Create ONE ultra-wide 3:1 3072x1024 pixel art background panorama for SUNFALL, 2D side-scrolling gothic action game. Reference 1 architectural mood only: cathedral exterior on cliff, oxidized teal spires, dark slate. Reference 2 exact game pixel-art density and quality: sharp deliberate small pixel clusters, blue teal atmospheric layers, detailed stone, amber lights. Original level: royal cathedral approach. Monumental cathedral fills middle/background with long side elevation, flying buttresses, broken narrow bridges, mist-filled ravine beneath, teal rooftops, remote towers. Camera side-on side-scroller, horizontal composition, NOT perspective road into distance. Foreground bottom 20% quiet dark mist and foundations behind gameplay. No playable platforms, no characters, no UI/text/logos, no framing, no collage. Cold blue slate palette, sparse warm windows, readable nuanced shadows. No photorealistic smooth rendering or blur. One continuous full-resolution panorama, no panels.

### cloister

ONE 3:1 ultra-wide 3072x1024 detailed pixel-art game background, SUNFALL gothic royal cathedral room2 cloister. Reference1 interior architectural mood, Reference2 match precise pixel density/cold teal navy stone rendering. Side-on horizontal elevation of vast candlelit cathedral aisle, repeated tall slender columns, pointed rib vaults, broken pale blue stained glass windows, shallow arches behind iron railing, sparse dark wine royal tapestries with tiny gold solar motifs, pools of warm candlelight, blue fog between columns. Camera orthographic side scroller; no deep central perspective vanishing road. All architecture midground, bottom20% quiet shaded foundations; no foreground pillars blocking playable actors. No characters, no playable floating platforms, no UI, text, logos. Individual crisp pixel clusters and stair-stepped contours not blurry painted photograph. One continuous panorama no panels. Dark but clear readable details, gold accent restrained.

### chapel

Generate ONE 3072x1024 ultra-wide 3:1 pixel-art background for SUNFALL stage3 room3 royal solar-seal chapel. References: first the cathedral interior mood; second sharp small pixel cluster style. Side-view horizontal gothic cathedral interior, cold deep navy carved stone, pointed arcades and blue stained glass, dark gold astronomical tracery, hanging bronze chains attached to masonry, dim amber candle clusters. Center slightly elevated distant ceremonial altar with a closed small circular solar reliquary and threadlike gold light inside cracks; not a bright sun disk. Shattered rib-vault spans entire width, successive wall bays, empty shadowed lower foreground. More enclosed and solemn than cloister, very little vegetation. No characters, no HUD, no lettering, no brands, no foreground obstacle columns, no playable platforms. Readable fine crisp pixel art consistent with reference2, no smeared blur, no photorealism. Single continuous panorama no atlas strips.

### sanctum

ONE ultra-wide 3:1 3072x1024 pixel art game background panorama. SUNFALL stage3 boss sanctuary, royal cathedral sun-binding inner sanctum. Reference1 interior cathedral architecture inspiration, reference2 crisp detailed pixel density. Original side-scrolling arena backdrop: monumental symmetrical vaulted apse, tall cold pale cyan glass lancet windows, carved blue-black stone columns at sides, torn ivory royal banners edged antique gold, enormous dark stone solar astrolabe embedded in rear wall centered upper middle, currently UNLIT, hair-thin golden seams, small amber candles, ominous depth in vaulted roof. Empty broad front central space for a boss. The floor is distant dark stone, quiet bottom20%; no characters, no figures/statues resembling bosses, no floating platforms, no UI or text. Side-on elevation suited horizontal 2D gameplay, not isometric. Strong layered pixel clusters, distinct tiny stone chips, no blur or photographic rendering. Sparse pale gold shafts from ceiling, restrained brightness leaves actor readable. One continuous full-res image no panels.

### platform

Create production-ready transparent PNG pixel-art platform strip for 2D gothic side scrolling game. Wide canvas3:1 target3072x1024. ONE continuous horizontally straight flat ledge centered vertically, occupies x1%..99%, y35%..72%. Entire surrounding canvas true alpha transparent. Precise side elevation, top walkable edge horizontal at35% height, thin pale worn bevel, dark blue-black cathedral masonry blocks below, carved antique-gold sun medallions at intervals, sparse cracks, tiny fallen ivory petals on top but NO grass. Underside irregular stone chips hangs slightly, no large pendant obstructions. Fine crisp pixel clusters matching provided forest screenshot reference quality, high detail consistent pixel density, hard pixels no blur. Keep exact same top level across full length for collider alignment. No characters, text, frames, environment, shadow rectangle or floor. Not multiple strips or sprites, just one long ledge.

### boss

Create transparent PNG production pixel-art sprite sheet for ORIGINAL gothic boss AUREL guardian of bound sun, SUNFALL 2D action game. Reference image is MOOD ONLY; substantially redesign silhouette and details. Design: faceless charcoal hood with two small amber eyes, asymmetrical cracked antique-gold eclipse halo above head, short torn ivory funeral stole over layered midnight-blue bronze plate and long split robe, SIX angular black stained-glass blade feathers arranged as modest folded wings, thin gold glowing cracks in glass. Holds short ceremonial sun glaive. No huge horn wings like reference. Not a copy. Crisp deliberate pixel clusters, limited navy/ivory/old gold palette, readable at 160px tall. Sheet exactly 4 columns x2 rows, target2048x1024 (eachcell512 square), eight equal square cells, generous transparent gutters, NO labels/borders/background. SAME character scale, hood size, body height and baseline across all frames, feet at y90% cell, center body x50%, silhouette fits eachcell with10% margins. Right-facing three-quarter side view for gameplay. Row1: idle1 wings folded; idle2 cloth subtly lifted; step1 rightfootforward; step2 leftfootforward. Row2: sweep windup glaive drawn left; sweep release glaive right (weapon inside cell, no huge effect arc); solar cast rightarm out with smallgold flame; eclipse channel both arms raised wingfeathers spread slightly. All eight full bodies visible, no cropping, consistent anatomy and exact same body size. TRUE transparency.

### portraits

Create SIX dialogue portraits of the SAME original gothic boss AUREL from attached sprite sheet. Transparent PNG atlas, exact3columns x2rows equal square cells, canvas1536x1024, each portrait within its own512square with20pixel transparent gutter. Waist/chest-up frontal slight3/4 bust, same framing, size and head center in everycell. Preserve identity: dark empty hood, tiny amber eyes, ivory frayed funeral stole with bone shoulder clasp, midnight plated robes and antique-gold trim, broken gold circular solar halo, angular black glass feathers behind shoulders. Painterly detailed gothic game dialogue illustration crisp clean edges, not pixelated facial closeup. Six emotional states in reading order:1 calm impassive stare;2 cold contempt headslightlytilted eyesnarrow;3 wrath eyesburninggold wingfeathersraised;4 startled eyeswide headrecoils;5 wounded hoodlowered goldcracks dim;6 resigned sorrow headbowed eyessoft haloslightlybroken. Communicate emotion via gaze, hood tilt, hands, halo glow; NO human face exposed and no change of character. Keep all heads and halos inside eachcell, no text labels, no borders, no scenery, true alpha transparency. Do not include weapon full body or sprite animations. Suitable for Korean dark fantasy story guardian of sun seal.


