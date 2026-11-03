# 대사창의 찢어진 하늘 일러스트

2026-11-01. 사용자가 첨부한 서장 대사창 왼쪽의 단순 직선/사선 임시 봉합 그림을 전용 환경 일러스트로 교체했습니다.

최종 이미지: `Assets/Sunfall/Resources/Story/StitchedSkyV1.png`.

내장 image_gen으로 새 이미지 1장을 생성했습니다. 원본은 `C:/Users/sakak/.codex/generated_images/01a17569-bfbf-7233-a71a-72eece5bde92/exec-b49bf97d-d435-46ba-885f-1f89f3d2ba5d.png`. 프로젝트에 수정 없이 복사했으며 CLI/API fallback을 사용하지 않았습니다. 사용자 스크린샷은 수정 위치를 확인하는 용도로 사용했고 생성 도구에 편집 대상으로 전달하지 않았습니다.

구성은 짙은 구름, 비대칭 하늘 균열, 별이 보이는 깊은 내부, 곡선으로 이어진 금빛 실, 안개 속 고딕 첨탑입니다. `StoryArtImporter`는 최대 1024px, Bilinear, Clamp, 무압축, Mipmap 끔으로 작은 대사창에서 매끄럽게 표시합니다.

`DrawStorySkyPortrait`는 기존 215×270 영역 안에 원본 비율을 유지한 이미지를 넣고, 얇은 금빛 테두리와 어두운 자막 영역을 제공합니다. 기존 임시 봉합 그림을 공유하던 해설/어머니/주민/전용 초상화가 없는 보스 대사에 적용됩니다. 기존 주인공·마을 NPC·후반 보스 전용 초상화 경로는 유지합니다. 타이틀 장식이나 전투 중 하늘 봉합 연출은 이번 수정 대상이 아닙니다.

## 생성 프롬프트

`transparent_background: false`, 참조 이미지 없음.

```text
Use case: stylized-concept. Asset type: finished narrative illustration for a dark gothic fantasy game's dialogue portrait panel, vertical 4:5 composition, opaque full bleed. Primary request: depict the sky itself torn open and partially sewn back together with luminous golden magical thread. Massive layered midnight blue and charcoal storm clouds form two organic uneven curling edges around a narrow jagged diagonal vertical rift. Within the rift is deep near-black violet cosmic depth with tiny distant stars and a restrained warm ivory-gold glow, NOT a bright lightning bolt. Only the lower third of the rift has 4 to 5 delicate curved interlaced golden sutures, attached convincingly to cloud edges; irregular spacing, tiny stitch holes, subtle hanging gold filament. Upper rift remains open. The stitches look like fine glowing silk, NOT a ladder, fish skeleton, zipper, railroad, straight parallel bars, or surgical skin wound. At the bottom fifth, a few small distant gothic cathedral spires and dark medieval roof silhouettes establish immense sky scale, faint amber windows in mist. Mood: melancholy, mysterious, beautiful dark fantasy; refined painterly game key art, rich atmospheric depth, controlled luminous edges, no excessive bloom. Strong readable silhouette and limited large shapes so the scene still reads at 190 by 210 pixels. Keep the main rift centered with 12 percent side safe margins, no cropped focal detail. Colors: ink navy, slate blue, desaturated violet, antique gold, small ivory highlights. No people, faces, hands, weapons, interface frame, text, letters, captions, logos or watermark. A complete polished environmental illustration, not a symbol on a flat background.
```

## 검증

생성된 이미지의 구성 확인 및 C# 컴파일만 수행합니다. 컴파일 로그는 `Temp/StorySkyArtCompile/compile.log`입니다. Unity Play·게임 QA·테스트 실행은 하지 않습니다.
