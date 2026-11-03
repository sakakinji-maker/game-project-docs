# 고딕 포탈·횃불 이미지 교체 (2026-10-28)

내장 image_gen으로 두 장의 투명 도트 시트를 생성했습니다. CLI/API fallback은 사용하지 않았습니다.

## 에셋

- `Assets/Sunfall/Resources/World/GothicPortalsV1.png`: 1535×1024, 왼쪽 붉은 봉인·사슬 / 오른쪽 청록 마법빛의 열린 포탈.
- `Assets/Sunfall/Resources/World/GothicTorchesV1.png`: 1536×1024, 왼쪽 주황 불꽃 / 오른쪽 붉은 마법 불꽃.
- 두 PNG의 알파 채널과 빈 영역의 투명도를 확인했습니다. 원본 파일을 변형하지 않고 런타임 UV로 각 부분을 표시합니다.
- 기존 WorldArtImporter의 Point / Clamp / 무압축 / 밉맵 없음 / NPOT 크기 유지 / 최대 4096 설정을 사용합니다.

## 적용

- 마을 성문 앞에 열린 포탈과 양옆 횃불을 배치했습니다. 배경에 그려진 건축 성문은 유지하고, 실제 상호작용 위치에 포탈 이미지를 겹쳐 표시합니다.
- 세 전투방과 보스방의 사각형 문·세로 봉인선을 새 포탈 이미지로 대체했습니다. 적이 살아 있으면 봉인 상태, 처치 조건을 달성하면 열린 상태로 표시합니다.
- 구역 안내 등불과 기존 월드 등불을 철제 횃불 이미지로 교체했습니다. 보스 지역은 붉은 불꽃을 사용합니다.
- 문 이동 규칙은 변경하지 않았습니다. 문 앞 ↑, 거리·지상 조건, 적 전원 처치 조건을 유지합니다.
- 두 포탈 상태의 받침 중심·발 기준과 표시 배율을 맞춰 개방 시 위치와 크기가 튀지 않도록 구성했습니다. 불꽃과 배경 광택은 이미지에 포함된 정적 표현입니다.
- 표시 코드: `Assets/Sunfall/Runtime/SunfallGame.WorldProps.cs`. 리소스 임포트 중 누락되면 재시도합니다.

## 확인

Core / Runtime / Editor / Tests 컴파일 성공. 오류·경고 출력 없음.
로그: `Temp/PortalTorchCompile/compile.log`.
Unity Play, 자동 테스트 실행, 실제 게임플레이 QA는 하지 않았습니다.

## 생성 프롬프트

### 포탈

```text
Use case: stylized-concept. Asset: production pixel-art sprite sheet of a gothic portal for SUNFALL, a sunless medieval 2D side-scrolling action game. Create TWO full-height portal sprites side by side, exactly two equal cells in one horizontal row, no other assets. Left cell: locked state, carved dark blue-grey stone pointed arch, small aged brass accents and extinguished sun crest, dark plum magical membrane inside, fine crimson rune seal with crossed spectral chains. Right cell: SAME gate silhouette, same dimensions and perfectly matched base/root position, now open with a luminous translucent turquoise magical veil, pale teal swirling wisps and warm gold accents, no chains. Architectural style: slender cathedral-like pointed stone arch with small spires, visibly detailed weathered pixel stone blocks, squat stone plinth flat on the ground, frontal orthographic side-scroller prop, no perspective. Outer canvas must be truly transparent; magic is only inside arch, soft sparse glow can extend a few pixels outside. Each complete gate occupies 80% cell width and 90% cell height, wide transparent gutters and margins, equal bottom baseline, nothing cropped. Each gate should read at around 110px wide and 165px tall in-game. Handcrafted crisp pixel clusters, hard outlines, limited blue charcoal/grey/aged gold palette; ornate but readable. No environment, floor plane, scenery, shadows outside plinth, people, text, letters, numbers, watermarks, UI or labels. Landscape sheet approximately 3:2 aspect, two equally sized isolated upright gates. Actual alpha transparency, not a painted checkerboard.
```

### 횃불

```text
Use case: stylized-concept. Asset: production pixel-art prop sprite sheet of TWO freestanding gothic torches for SUNFALL, a sunless medieval 2D side-scrolling action game. Exactly two equal cells side by side in one horizontal row. Left: amber-gold orange fire burning in a dark wrought-iron gothic standing brazier/torch with pointed iron claws around the bowl, slender iron shaft, modest decorative brass collar, sturdy flat three-foot base. Right: IDENTICAL iron standing torch, same dimensions and perfectly matched baseline, but with crimson-pink magical flame for a demon sanctuary. Front orthographic side view for a side-scroller, complete objects including tips of fire and base, no perspective. Clear elongated silhouette, realistic prop scale: flame+cup is upper third, long shaft below. Each full prop centered in its own cell, 70% cell width, 85% cell height, generous transparent margins and clear gap between cells, no cropped flames. Detailed handcrafted crisp pixel clusters and hard edges, dark slate iron and aged brass, bright yellow-white flame center left, pale pink-white center right. The flame itself has real translucent alpha edge glow, no enclosing backdrop. Readable at about 34px wide by 78px tall in game. No scenery, floor plane, rectangular background, people, text, letters, numbers, watermark, UI or labels. Landscape sheet about 3:2 aspect, actual transparent alpha around both isolated props, not a painted checkerboard.
```
