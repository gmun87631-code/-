(function () {
  const canvas = document.getElementById("gameCanvas");
  const overlay = document.getElementById("overlay");
  const overlayTitle = document.getElementById("overlayTitle");
  const overlayText = document.getElementById("overlayText");
  const scoreLabel = document.getElementById("scoreLabel");
  const healthLabel = document.getElementById("healthLabel");
  const crystalLabel = document.getElementById("crystalLabel");
  const startButton = document.getElementById("startButton");
  const restartButton = document.getElementById("restartButton");

  const ui = {
    setHud(score, health, crystalsLeft) {
      scoreLabel.textContent = `Score: ${score}`;
      healthLabel.textContent = `Health: ${health}`;
      crystalLabel.textContent = `Crystals: ${crystalsLeft}`;
    },
    showOverlay(title, text) {
      overlayTitle.textContent = title;
      overlayText.textContent = text;
      overlay.hidden = false;
    },
    hideOverlay() {
      overlay.hidden = true;
    }
  };

  const game = new window.PlatformerGame(canvas, ui, window.GameConfig);

  const keyMap = {
    ArrowLeft: "left",
    KeyA: "left",
    ArrowRight: "right",
    KeyD: "right"
  };

  window.addEventListener("keydown", (event) => {
    if (keyMap[event.code]) {
      game.input[keyMap[event.code]] = true;
    }
    if (event.code === "Space" || event.code === "KeyW" || event.code === "ArrowUp") {
      if (!event.repeat) {
        game.input.jumpPressed = true;
      }
      event.preventDefault();
    }
  });

  window.addEventListener("keyup", (event) => {
    if (keyMap[event.code]) {
      game.input[keyMap[event.code]] = false;
    }
    if (event.code === "Space" || event.code === "KeyW" || event.code === "ArrowUp") {
      game.input.jumpReleased = true;
      event.preventDefault();
    }
  });

  startButton.addEventListener("click", () => game.start());
  restartButton.addEventListener("click", () => game.restart());

  let previous = performance.now();
  function frame(now) {
    const dt = Math.min((now - previous) / 1000, 1 / 30);
    previous = now;
    game.update(dt);
    game.render();
    requestAnimationFrame(frame);
  }

  ui.showOverlay("Ready", "Press Start to enter Starfall Crossing in your browser.");
  game.render();
  requestAnimationFrame(frame);
}());
