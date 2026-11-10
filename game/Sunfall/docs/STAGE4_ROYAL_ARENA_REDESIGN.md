# 4스테이지 왕실 회랑·결투장 / 바르칸 교체

2026-11-04. 본인 외관 연결길 자율 제작, 회랑 사진, 투기장 사진, 검은 기사 사진 기준.

## 경로와 배경
3스테이지 종료 후 기존 포탈 진행으로 4스테이지 첫 방 진입:
대성당 뒤편 요새길 → 역광의 왕실 회랑 → 심장 봉인 회랑 → 왕관의 결투장.
첫 방은 대성당을 뒤로 두고 왕실 요새로 이어지는 다리.
회랑은 꽃 모양 리브 천장, 푸른 석재 아치, 발코니와 체크 바닥을 옆으로 보는 게임 화면으로 재구성.
결투장은 2층 관람석, 중앙 왕실 관람대, 화로와 붉은 장막.
기존 임시 도형 기둥/톱니/다이아몬드 심장과 공용 배경을 교체.
발판은 3스테이지의 고해상도 왕실 석재를 재사용하여 연결감을 유지.
충돌 지형·몹 배치·스테이지 이동 조건·저장은 변경하지 않음.

## 보스
바르칸 이름과 심장지기 설정 유지. 검은 닫힌 투구, 중갑, 찢어진 검은 망토, 붉은 흉부 핵.
참고 이미지의 사슬추 무기는 긴 자루의 양손 전투망치로 변경.
기본 시트 8자세 + 망치 전용 시트 8프레임 + 대화 6표정.
HammerSweep의 기존 .8초 준비(2페이즈)/1.05초 준비(1페이즈), .55초 발동 잔여 타이머에 동기화.
준비 프레임 0~3, 피해 발생 직후 접촉 프레임4, 회수5~7.
몸통/투구/발 기준으로 기본 배율 .34, 여백 보완 공격 시트 .435를 보정. 프레임별 자동 크기 맞춤 없음.
공격 시트는 최초 생성에서 잘린 망치를 내장 이미지 편집으로 복원한 여백 보완본 사용.
피해량·범위·패턴 순서·HP·보상은 유지. 망치 타격 파편 및 기존 범위의 지면 충격파, 새 회전 톱니 탄 이미지.
초상화 평상시/냉소/분노/놀람/부상/체념 연결.
도입 설명과 망치를 든 상대에게 맞는 짧은 대사 수정. 이야기 시청 여부 초기화 없음.

## 심장 장치
왕관 고리 / 루비 핵 / 회전 톱니 / 부서진 고리 별도 셀.
봉인 회랑과 결투장에 표시. 전투 중 맥동, 처치 후 핵 소멸과 깨진 고리, 금빛 봉합.
GUI 렌더링에서 상태를 전진시키지 않으며 visualClock/기존 처치 타이머 사용.

## 리소스 (Unity 프로젝트 기준)
| 경로 | 크기 |
|---|---|
| Assets/Sunfall/Resources/World/Stage4CausewayHDV2.png | 2172×724 |
| Assets/Sunfall/Resources/World/Stage4CloisterHDV2.png | 2172×724 |
| Assets/Sunfall/Resources/World/Stage4SealGalleryHDV2.png | 2172×724 |
| Assets/Sunfall/Resources/World/Stage4RoyalArenaHDV2.png | 2172×724 |
| Assets/Sunfall/Resources/World/Stage4HeartMechanismV2.png | 1254×1254 |
| Assets/Sunfall/Resources/Demons/Stage4VarkanKnightV2.png | 1774×887 |
| Assets/Sunfall/Resources/Demons/Stage4VarkanHammerV2.png | 1774×887 |
| Assets/Sunfall/Resources/Story/BossPortraits/VarkanKnightExpressionsV2.png | 1536×1024 |

배경 독립 이미지, 표시 폭2070, 비율 유지, 낮은 해상도 아틀라스 확대 없음.
Point / Clamp / 무압축 / 밉맵 없음 / NPOT 변환 없음. 대화 초상화만 Bilinear.
이전 리소스는 보존하고 새 이름으로 연결.

## 검증
tools/Compile-AttackArt.ps1 -OutputFolder Temp/Stage4RoyalArenaCompile
Core / Runtime / Editor / Tests 소스 컴파일 모두 exit=0.
로그: Temp/Stage4RoyalArenaCompile/compile.log.
8개 이미지 해상도·임포트 한도·GUID 확인.
망치 8프레임의 불투명 도형이 셀 경계에서 15px 이상 떨어져 있음을 확인.
플레이 / Unity Play / 테스트 실행 / QA는 하지 않음.
본인 확인: F4 첫 방, 3층 처치 후 진입 동선, 망치 공격 발 위치와 타격 순간, 보스 대화 표정, 처치 후 장치.

## 최종 프롬프트
내장 image_gen 개별 생성/편집 사용. CLI/API 대체 미사용.
원본은 Codex generated_images 보존, 선택된 최종본을 위 Resources에 복사.
### approach

ONE ultra-wide 3:1 3072x1024 crisp detailed pixel-art background panorama for SUNFALL 2D side-scrolling dark gothic game. Use attached stage3 artwork ONLY as architectural and pixel-density continuity reference. NEW stage4 first room: exiting behind this teal-roofed cathedral onto a long ruined elevated royal fortress causeway. Cathedral rear buttresses recede on LEFT, narrow stone connecting bridge and repeated balustrades across middle, massive royal fortress with closed pointed portal on RIGHT. Cold midnight blue slate and silver moonlike ambient glow, fog beneath bridge, restrained amber braziers and dark wine banners. No actual moon/sun needed. Original composition, side elevation appropriate for horizontal platformer, NO perspective road vanishing down center. Bottom20% quiet dark stone foundation and mist behind gameplay, NO foreground structures obscuring characters. Sharp fine pixel clusters, richly chipped masonry same quality as reference. No characters, UI, text, logo, panels or playable floating platforms. One continuous panorama.

### cloister

ONE 3:1 ultra-wide 3072x1024 crisp detailed pixel-art panorama SUNFALL stage4 royal cloister. Reference1 architecture and palette: intricate flower-shaped rib vaults, blue-silver pale stone, tall pointed open arches, elegantly carved balustrade, blue night beyond, worn polished dark checker stone. Reference2 existing game pixel art density, use sharp tiny deliberate pixel clusters. Recompose as a continuous SIDE-ON horizontal side-scrolling game background with repeated bays across frame, not perspective tunnel into distance. Behind arches distant royal towers in blue fog. Thin antique-gold channels in the stone subtly carry stolen light toward right, sparse amber walllamps. No characters, floating platforms, UI, lettering, lensflare circles or sparkles likephoto. Quiet bottom20% dark masonry behind playable terrain; no huge foreground columns obstructing actor. Architectural details readable, cold silver/navy palette, no blur or smooth 3Drender. Single fullresolution landscape, no panels.

### gallery

ONE 3072x1024 ultra-wide3:1 pixel art panorama for SUNFALL stage4 third combatroom. Reference1 flower-rib vaulted royal blue stone corridor; reference2 detailed crisp pixel game style. NEW room: enclosed royal processional gallery before forbidden arena, high floral pointed vaults, silver-blue carved clustered pillars, dark stained-glass lancets and deep wine velvet hanging banners, bronze sun conduits in wall, heavy central iron portcullis framed by stone crown relief, worn black blue checker floor. Sparse amber braziers grow warmer nearer entrance, coldblue side shadows. Arrange SIDE-ON horizontal elevation for side scrolling; no deep perspective tunnel, no giant foregroundpillar. No characters or playable platforms, no text or HUD. Keep bottom20% quieter shadowed masonry. Original architecture, sharp small pixel clusters, same pixel density as attached gameart. 3:1 single continuous panorama no separatepanels. No central giant floating heart, will be rendered separately.

### arena

ONE ultra-wide3:1 3072x1024 pixel-art panoramic boss arena background for SUNFALL. Reference1 provides arena composition/mood ONLY: enclosed gothic royal fighting pit, two tiers of arcaded stone spectator balconies, many amber braziers, central high royal box and dark crimson curtains. Reference2 matches sharp detailed pixel style/density. Recompose into a SIDE-ON horizontal 2D game arena, eye level with fightingfloor, NOT overhead oval and no foreground stairwalls. Wide empty flat arena at bottom quarter, upper threequarters rich dark blue-black vaulted stone hall and spectators as indistinct distant shadows, no detailed foreground people. Central elevated balcony has EMPTY dark pointed alcove reserved for separate heart-seal prop, no throne occupant, no boss. Dark burnt bronze trim, ivory worn stone arena edge, warm amber lamps, wine velvet, cool blue shadows. Clear gameplay background with restrained contrast at bottom. No UI,text,logo,characters, floating platforms or panels. Fine deliberate pixelclusters not smudged photoreal art. Single continuous fullresolution panorama.

### bossBase

Production transparent PNG sprite sheet, original SUNFALL boss VARKAN redesigned with reference's dark armored knight mood. 4columns x2rows EXACT equal square cells, wide2:1 target2048x1024. Eight FULLBODY right-facing3/4 sideview pixelart sprites. Design: tall heavy but athletic BLACKENED STEEL knight, CLOSED narrow slit greathelm with ridged crown brow (NO huge horns), tiny ember eyes, layered pointed armor with sparse tarnished silver/bronze edging, tattered long charcoal cape, broken chain at belt, small caged ruby ember in chest. Weapon is ONE rigid LONG-HANDLED TWO-HANDED WARHAMMER with large rectangular iron hammerhead and short backspike, NOT flail, chain weapon, axe or sword. Distinct original armor fromreference, same character in allframes. High-quality crisp deliberate pixelclusters navy-black steel highlights, readable silhouette for game sprite height140px. Body height helmettoptofeet about340pixels within nominal512cell, same in ALLframes, footbaseline90%cell, hips x50%, at least6%transparent gutters, allhammer/cape fitcell. Row1 idle1,idle2,walkleftstep,walkrightstep. Row2 hammerready angledbehindshoulder, hammerlow followthroughright, leftpalm casting amber sigil with hammer heldotherhand, enragedchestglowing pose. No labels, gridlines, scenery, floor, background or castshadow. True alpha0 aroundsprites, no cutoffs.

### hammer

Create transparent PNG animation sheet of EXACT SAME Varkan knight from reference sprite sheet, one smooth two-handed WARHAMMER SWING animation eight consecutive keyframes. Layout4columns2rows target2048x1024 SAME shape/resolution and character pixel size asreference, do notshrink sprites. Eachcell samefullbody humanoidarmor proportions and helmet size asreference idle; feetbaseline95%cell, pelvisrootx52%cell; fixedscale allframes. Right-facing3/4 gameplay. Blacksteel closed greathelm tinyember eyes, ruby chestcage, torn charcoal cape, long rigid shaft rectangular ironhammer withshortbackspike. Row1 frames0readywithhammerupright,1shoulderscoilhammerdrawnback,2deepwinduphammerraisedbehindhead,3downswingdiagonal towardRIGHT beforecontact. Row2 frames4hammerhitsgroundonRIGHT legsbraced,5lowfollowthroughcapeforward,6hammerpullbackup,7returnready. Helm and armorsameasreference NO redesign. Bendknees and torso for poses without scaling anatomy. Oversizedhammer remains fully ineachcell, leave cleartransparentgutter at boundaries; capecontained too. No floating specialeffects excepttiny pale short motionstreak ondownswing, no explosion bakedintoart. Crisp highquality detailedpixelart, no labels/grid/background/shadows. Transparent alpha0. Frame4 contact is core of animation, dramatic weighted swing with bothhands.

### hammerPadding

EDIT attached animation atlas, preserve this SAME Varkan knight and eight hammer-swing poses and exact4columns2rows structure. Fix clipped weapons and margins. Uniformly REDUCE ALL eight full characters to85% current size within their own cells, keeping same armor/helmet dimensions relative to eachother, and center each at x50% feetbaseline90%. Restore the complete hammerhead clipped above frame2 (thirdcell top row) and clipped right of frame3 (fourthcell top row). ALL hammerheads, cape tips, feet and motionarc must have minimum20pixels true transparent margin from EACH cell edge on a1774x887 atlas. No silhouettes may touch neighboringcells. Transparent output wide2:1 target2048x1024, exactequal4x2 grid. Row1 ready,coil,raisedwindup,downswing; row2 contacthammergroundright,followthrough,recover,ready. Preserve coherent movement and right-facing knight, identical rigid rectangular iron warhammer. NO background, checkerboard, labels or panelborders. Crisp pixelart. This is a production padding repair, do not redesign armor or rearrangeposes. Complete restored weapon is essential.

### portraits

Generate SIX matching dialogue portraits of EXACT Varkan knight identity from attached base sprites. True transparent PNG atlas3columns2rows,1536x1024, six equal square512cells. Chest-up front3/4 bust at consistent headcenter andscale everycell,20pxgutters, no fullbody. Closed blackenedsteel ridged greathelm narrow ember eye slits, NO HORNS, layered pointed pauldrons, weathered charcoal cape, small caged red emberheart at sternum, restrained antiquebronze trim, hammerhaft atshoulder optional butnocutoffs. Detailed darkfantasy illustrated portraits suitable dialogue, crisp edges. Readingorder expressions: neutral stoic; contempt slightheadtilt; enraged red eyesbright fistclenched; shocked headrecoil eyeswideglow; wounded dented armor headlowered; resigned embersdim chinbowed. Emotion throughhelmettilt/eyeglow/handpose, NEVER revealhumanface. Allsameknight notsixdesigns. No text,labels,backgroundcards,scenery or ornamentalframe. Keepwholehelmetinsquare, face clearlylit readable atsmallportraitsize.

### heartProps

Transparent PNG pixel-art prop atlas for SUNFALL royal gothic arena, exact2columns2rows squarecanvas1536x1536, equal squarecells. Four SEPARATE centered isolated components intended for code animation. Top-left: ornate black iron and tarnished gold CIRCULAR CROWN SEAL FRAME, radius35%cell, pointed crown at12oclock, EMPTY transparent circular center, carved suntracery, no jewel and no chains. Top-right: deep RUBY faceted heart-shaped magical CRYSTAL, nonanatomical, fine gold glowing thread trapped inside, small bronze cage ribs onedges, centered, occupies50%cellheight, restrained redglow, truealphaoutside. Bottom-left: single ancient bronze GEAR ring, 16 clearly shaped teeth, emptytransparent center, subtle embossed runes, radius35%cell, perfectcenter forrotation. Bottom-right: BROKEN version of top-left crown ring, samealignment/scale, missing upperright segment, cracks and extinguisheddarkgold, nofloatingdebrisoutsidecell. Crisp tiny pixelclusters, chipped darkmetal and warmsparse highlights, matching highquality gothic2D game sprites. NO background scenery, no labels, no tile borders, no shadows, no huge glowfog fillingcells. Eachcell15%transparent margin.


