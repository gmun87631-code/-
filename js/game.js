(function () {
  const { Player, Enemy, Collectible, GoalPortal, rectsOverlap } = window.GameEntities;

  class PlatformerGame {
    constructor(canvas, ui, config) {
      this.canvas = canvas;
      this.ctx = canvas.getContext("2d");
      this.ui = ui;
      this.config = config;

      this.cameraX = 0;
      this.running = false;
      this.won = false;
      this.score = 0;
      this.input = createInputState();
      this.lastTime = 0;

      this.resetWorld();
    }

    resetWorld() {
      const level = this.config.level;
      this.player = new Player(this.config.player, level.spawn);
      this.platforms = level.platforms.map((platform) => ({ ...platform }));
      this.collectibles = level.collectibles.map((item) => new Collectible(item, this.config.collectible));
      this.enemies = level.enemies.map((enemy) => new Enemy(this.config.enemy, enemy));
      this.goal = new GoalPortal(this.config.goal, level.goal);
      this.score = 0;
      this.won = false;
      this.running = false;
      this.cameraX = 0;
      this.updateUi();
    }

    start() {
      this.resetWorld();
      this.running = true;
      this.ui.showOverlay("Stage Start", "Reach the sun gate, collect crystals, and stomp Ember Crawlers from above.");
      window.setTimeout(() => {
        if (this.running) {
          this.ui.hideOverlay();
        }
      }, 900);
    }

    restart() {
      this.start();
    }

    update(dt) {
      if (!this.running) {
        return;
      }

      this.player.update(dt, this.input, this, this.config.gravity);
      this.updateEnemies(dt);
      this.collectCrystals();
      this.checkHazards();
      this.checkGoal();
      this.updateCamera(dt);
      this.updateUi();
      this.input.jumpPressed = false;
      this.input.jumpReleased = false;
    }

    updateEnemies(dt) {
      for (const enemy of this.enemies) {
        enemy.update(dt);
        if (!enemy.alive) {
          continue;
        }

        if (!rectsOverlap(this.player.bounds, enemy.bounds)) {
          continue;
        }

        const playerBottom = this.player.y + this.player.height;
        const enemyTop = enemy.y;
        const stomping = this.player.vy > 0 && playerBottom - enemyTop < 22;

        if (stomping) {
          enemy.alive = false;
          this.player.vy = -this.config.player.stompBounce;
          this.score += this.config.enemy.stompBonus;
          continue;
        }

        const direction = this.player.x + this.player.width / 2 < enemy.x + enemy.width / 2 ? -1 : 1;
        const damaged = this.player.takeHit(
          direction * this.config.enemy.contactKnockbackX,
          this.config.enemy.contactKnockbackY
        );

        if (damaged && this.player.health <= 0) {
          this.lose("The Sky Explorer ran out of health.");
        }
      }
    }

    collectCrystals() {
      const playerCenterX = this.player.x + this.player.width / 2;
      const playerCenterY = this.player.y + this.player.height / 2;

      for (const crystal of this.collectibles) {
        if (crystal.collected) {
          continue;
        }

        const dx = playerCenterX - crystal.x;
        const dy = playerCenterY - crystal.y;
        const range = crystal.radius + Math.max(this.player.width, this.player.height) * 0.35;
        if (dx * dx + dy * dy <= range * range) {
          crystal.collected = true;
          this.score += crystal.value;
        }
      }
    }

    checkHazards() {
      for (const platform of this.platforms) {
        if (platform.type !== "hazard") {
          continue;
        }
        if (rectsOverlap(this.player.bounds, platform)) {
          this.lose("You fell into a molten gap.");
          return;
        }
      }

      if (this.player.y > this.config.floorDeathY) {
        this.lose("You fell out of the level.");
      }
    }

    checkGoal() {
      if (rectsOverlap(this.player.bounds, this.goal.bounds)) {
        this.running = false;
        this.won = true;
        this.ui.showOverlay(
          "Stage Clear",
          `Final Score: ${this.score}\nHealth Remaining: ${this.player.health}\nPress Restart to play again.`
        );
      }
    }

    lose(message) {
      this.running = false;
      this.won = false;
      this.ui.showOverlay("Try Again", `${message}\nPress Restart to run the level again.`);
    }

    updateCamera(dt) {
      const desired = clamp(
        this.player.x - this.canvas.width * 0.4 + this.player.facing * this.config.cameraLead,
        0,
        this.config.level.width - this.canvas.width
      );
      const blend = 1 - Math.pow(1 - this.config.cameraSmoothing, dt * 60);
      this.cameraX += (desired - this.cameraX) * blend;
    }

    updateUi() {
      this.ui.setHud(
        this.score,
        this.player.health,
        this.collectibles.filter((item) => !item.collected).length
      );
    }

    render() {
      const ctx = this.ctx;
      ctx.clearRect(0, 0, this.canvas.width, this.canvas.height);
      drawBackground(ctx, this.canvas, this.cameraX);
      ctx.save();
      ctx.translate(-Math.floor(this.cameraX), 0);
      this.drawPlatforms(ctx);
      this.drawHiddenPathHints(ctx);
      this.drawGoal(ctx);
      this.drawCollectibles(ctx);
      this.drawEnemies(ctx);
      this.drawPlayer(ctx);
      ctx.restore();
    }

    drawPlatforms(ctx) {
      for (const platform of this.platforms) {
        ctx.fillStyle = platform.type === "hazard"
          ? "#ff7445"
          : platform.hiddenPath
            ? "#95d5ff"
            : "#345c88";
        ctx.fillRect(platform.x, platform.y, platform.w, platform.h);

        if (platform.type === "hazard") {
          ctx.fillStyle = "rgba(255, 210, 120, 0.5)";
          for (let x = platform.x; x < platform.x + platform.w; x += 24) {
            ctx.beginPath();
            ctx.arc(x + 12, platform.y + 16, 8, 0, Math.PI * 2);
            ctx.fill();
          }
        }
      }
    }

    drawHiddenPathHints(ctx) {
      ctx.strokeStyle = "rgba(255,255,255,0.16)";
      ctx.lineWidth = 2;
      for (const platform of this.platforms) {
        if (!platform.hiddenPath) {
          continue;
        }
        ctx.strokeRect(platform.x, platform.y, platform.w, platform.h);
      }
    }

    drawCollectibles(ctx) {
      for (const crystal of this.collectibles) {
        if (crystal.collected) {
          continue;
        }
        ctx.fillStyle = this.config.collectible.color;
        ctx.beginPath();
        ctx.moveTo(crystal.x, crystal.y - crystal.radius);
        ctx.lineTo(crystal.x + crystal.radius, crystal.y);
        ctx.lineTo(crystal.x, crystal.y + crystal.radius);
        ctx.lineTo(crystal.x - crystal.radius, crystal.y);
        ctx.closePath();
        ctx.fill();
      }
    }

    drawEnemies(ctx) {
      for (const enemy of this.enemies) {
        if (!enemy.alive) {
          continue;
        }

        ctx.strokeStyle = "rgba(32, 46, 69, 0.22)";
        ctx.lineWidth = 3;
        ctx.beginPath();
        ctx.moveTo(enemy.minX + enemy.width * 0.5, enemy.y + enemy.height + 8);
        ctx.lineTo(enemy.maxX + enemy.width * 0.5, enemy.y + enemy.height + 8);
        ctx.stroke();

        ctx.fillStyle = "rgba(32, 46, 69, 0.28)";
        ctx.beginPath();
        ctx.arc(enemy.minX + enemy.width * 0.5, enemy.y + enemy.height + 8, 4, 0, Math.PI * 2);
        ctx.arc(enemy.maxX + enemy.width * 0.5, enemy.y + enemy.height + 8, 4, 0, Math.PI * 2);
        ctx.fill();

        ctx.fillStyle = enemy.color;
        roundRect(ctx, enemy.x, enemy.y, enemy.width, enemy.height, 12, true);
        ctx.fillStyle = "#202e45";
        if (enemy.direction >= 0) {
          ctx.fillRect(enemy.x + 11, enemy.y + 9, 5, 5);
          ctx.fillRect(enemy.x + enemy.width - 12, enemy.y + 10, 4, 4);
        } else {
          ctx.fillRect(enemy.x + 8, enemy.y + 10, 4, 4);
          ctx.fillRect(enemy.x + enemy.width - 16, enemy.y + 9, 5, 5);
        }
      }
    }

    drawGoal(ctx) {
      ctx.fillStyle = this.config.goal.color;
      roundRect(ctx, this.goal.x, this.goal.y, this.goal.w, this.goal.h, 16, true);
      ctx.fillStyle = "rgba(255,255,255,0.38)";
      ctx.fillRect(this.goal.x + 10, this.goal.y + 10, this.goal.w - 20, this.goal.h - 20);
    }

    drawPlayer(ctx) {
      if (this.player.invulnerableTimer > 0 && Math.floor(this.player.invulnerableTimer * 12) % 2 === 0) {
        return;
      }

      ctx.fillStyle = this.config.player.color;
      roundRect(ctx, this.player.x, this.player.y, this.player.width, this.player.height, 12, true);
      ctx.fillStyle = "#ffffff";
      ctx.fillRect(this.player.x + 8, this.player.y + 10, 6, 6);
      ctx.fillRect(this.player.x + this.player.width - 14, this.player.y + 10, 6, 6);
      ctx.fillStyle = "#1b3658";
      ctx.fillRect(this.player.x + 12, this.player.y + 28, this.player.width - 24, 6);
    }
  }

  function createInputState() {
    return {
      left: false,
      right: false,
      jumpPressed: false,
      jumpReleased: false
    };
  }

  function drawBackground(ctx, canvas, cameraX) {
    const gradient = ctx.createLinearGradient(0, 0, 0, canvas.height);
    gradient.addColorStop(0, "#7dd8ff");
    gradient.addColorStop(1, "#eafaff");
    ctx.fillStyle = gradient;
    ctx.fillRect(0, 0, canvas.width, canvas.height);

    for (let i = 0; i < 8; i += 1) {
      const x = ((i * 220) - cameraX * 0.2) % (canvas.width + 160);
      ctx.fillStyle = "rgba(255,255,255,0.75)";
      ctx.beginPath();
      ctx.arc(x, 90 + (i % 3) * 26, 30, 0, Math.PI * 2);
      ctx.arc(x + 28, 88 + (i % 3) * 26, 24, 0, Math.PI * 2);
      ctx.arc(x + 56, 92 + (i % 3) * 26, 20, 0, Math.PI * 2);
      ctx.fill();
    }

    ctx.fillStyle = "#8bd38a";
    ctx.fillRect(0, canvas.height - 56, canvas.width, 56);
  }

  function roundRect(ctx, x, y, width, height, radius, fill) {
    ctx.beginPath();
    ctx.moveTo(x + radius, y);
    ctx.arcTo(x + width, y, x + width, y + height, radius);
    ctx.arcTo(x + width, y + height, x, y + height, radius);
    ctx.arcTo(x, y + height, x, y, radius);
    ctx.arcTo(x, y, x + width, y, radius);
    ctx.closePath();
    if (fill) {
      ctx.fill();
    }
  }

  function clamp(value, min, max) {
    return Math.max(min, Math.min(max, value));
  }

  window.PlatformerGame = PlatformerGame;
}());
