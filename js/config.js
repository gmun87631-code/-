(function () {
  const GameConfig = {
    canvasWidth: 960,
    canvasHeight: 540,
    gravity: 2100,
    floorDeathY: 780,
    cameraSmoothing: 0.12,
    cameraLead: 130,
    player: {
      name: "Sky Explorer",
      width: 34,
      height: 50,
      color: "#29b7ff",
      moveSpeed: 320,
      acceleration: 2100,
      deceleration: 2400,
      jumpForce: 820,
      doubleJumpForce: 780,
      maxAirJumps: 1,
      jumpCutMultiplier: 0.42,
      maxHealth: 3,
      stompBounce: 500
    },
    enemy: {
      width: 36,
      height: 28,
      color: "#ef6a41",
      speed: 110,
      patrolDistance: 140,
      contactKnockbackX: 360,
      contactKnockbackY: 320,
      stompBonus: 150
    },
    collectible: {
      radius: 12,
      color: "#6ffff1"
    },
    goal: {
      width: 40,
      height: 76,
      color: "#ffd34f"
    },
    level: {
      width: 2600,
      spawn: { x: 100, y: 370 },
      goal: { x: 2450, y: 338 },
      platforms: [
        { x: 0, y: 432, w: 300, h: 108, type: "ground" },
        { x: 356, y: 432, w: 220, h: 108, type: "ground" },
        { x: 645, y: 432, w: 180, h: 108, type: "ground" },
        { x: 885, y: 432, w: 210, h: 108, type: "hazard" },
        { x: 1180, y: 432, w: 280, h: 108, type: "ground" },
        { x: 1545, y: 432, w: 260, h: 108, type: "ground" },
        { x: 1885, y: 432, w: 220, h: 108, type: "ground" },
        { x: 2185, y: 432, w: 260, h: 108, type: "ground" },
        { x: 240, y: 318, w: 110, h: 22, type: "platform" },
        { x: 430, y: 254, w: 120, h: 20, type: "platform" },
        { x: 630, y: 202, w: 120, h: 20, type: "platform" },
        { x: 1070, y: 306, w: 150, h: 20, type: "platform" },
        { x: 1280, y: 238, w: 130, h: 20, type: "platform" },
        { x: 1460, y: 170, w: 110, h: 20, type: "platform" },
        { x: 1630, y: 220, w: 110, h: 20, type: "platform" },
        { x: 1765, y: 280, w: 100, h: 20, type: "platform" },
        { x: 2005, y: 332, w: 120, h: 20, type: "platform" },
        { x: 2140, y: 272, w: 90, h: 20, type: "platform" },
        { x: 2270, y: 214, w: 110, h: 20, type: "platform" },
        { x: 1110, y: 120, w: 120, h: 18, type: "platform", hiddenPath: true },
        { x: 1265, y: 86, w: 110, h: 18, type: "platform", hiddenPath: true },
        { x: 1410, y: 58, w: 100, h: 18, type: "platform", hiddenPath: true }
      ],
      collectibles: [
        { x: 280, y: 282, value: 50 },
        { x: 488, y: 220, value: 75 },
        { x: 685, y: 168, value: 100 },
        { x: 1128, y: 272, value: 75 },
        { x: 1338, y: 202, value: 100 },
        { x: 1495, y: 132, value: 125 },
        { x: 1165, y: 84, value: 200, hiddenPath: true },
        { x: 1318, y: 48, value: 200, hiddenPath: true },
        { x: 1460, y: 20, value: 250, hiddenPath: true },
        { x: 2038, y: 296, value: 100 },
        { x: 2325, y: 178, value: 150 }
      ],
      enemies: [
        { x: 470, y: 404, patrolDistance: 90 },
        { x: 720, y: 174, patrolDistance: 70 },
        { x: 1330, y: 404, patrolDistance: 100 },
        { x: 1605, y: 404, patrolDistance: 90 },
        { x: 2055, y: 304, patrolDistance: 70 }
      ]
    }
  };

  window.GameConfig = GameConfig;
}());
