(function () {
  function rectsOverlap(a, b) {
    return (
      a.x < b.x + b.w &&
      a.x + a.w > b.x &&
      a.y < b.y + b.h &&
      a.y + a.h > b.y
    );
  }

  class Player {
    constructor(config, spawn) {
      this.config = config;
      this.width = config.width;
      this.height = config.height;
      this.spawn(spawn);
    }

    spawn(spawn) {
      this.x = spawn.x;
      this.y = spawn.y;
      this.vx = 0;
      this.vy = 0;
      this.grounded = false;
      this.facing = 1;
      this.health = this.config.maxHealth;
      this.invulnerableTimer = 0;
      this.airJumpsRemaining = this.config.maxAirJumps;
    }

    get bounds() {
      return { x: this.x, y: this.y, w: this.width, h: this.height };
    }

    update(dt, input, world, gravity) {
      if (this.invulnerableTimer > 0) {
        this.invulnerableTimer -= dt;
      }

      const moveAxis = (input.right ? 1 : 0) - (input.left ? 1 : 0);
      if (moveAxis !== 0) {
        this.facing = moveAxis;
      }

      const target = moveAxis * this.config.moveSpeed;
      const accel = moveAxis === 0 ? this.config.deceleration : this.config.acceleration;
      this.vx = moveTowards(this.vx, target, accel * dt);

      if (input.jumpPressed) {
        if (this.grounded) {
          this.vy = -this.config.jumpForce;
          this.grounded = false;
          this.airJumpsRemaining = this.config.maxAirJumps;
        } else if (this.airJumpsRemaining > 0) {
          this.vy = -this.config.doubleJumpForce;
          this.airJumpsRemaining -= 1;
        }
      }

      if (input.jumpReleased && this.vy < 0) {
        this.vy *= this.config.jumpCutMultiplier;
      }

      this.vy += gravity * dt;

      this.x += this.vx * dt;
      resolveHorizontal(this, world.platforms);

      this.y += this.vy * dt;
      this.grounded = false;
      resolveVertical(this, world.platforms);

      if (this.grounded) {
        this.airJumpsRemaining = this.config.maxAirJumps;
      }
    }

    takeHit(knockbackX, knockbackY) {
      if (this.invulnerableTimer > 0) {
        return false;
      }

      this.health -= 1;
      this.vx = knockbackX;
      this.vy = -knockbackY;
      this.invulnerableTimer = 1.0;
      return true;
    }
  }

  class Enemy {
    constructor(base, override) {
      this.width = base.width;
      this.height = base.height;
      this.color = base.color;
      this.speed = base.speed;
      this.patrolDistance = override.patrolDistance || base.patrolDistance;
      this.x = override.x;
      this.y = override.y;
      this.startX = override.x;
      this.minX = override.x - this.patrolDistance;
      this.maxX = override.x + this.patrolDistance;
      this.direction = 1;
      this.alive = true;
    }

    get bounds() {
      return { x: this.x, y: this.y, w: this.width, h: this.height };
    }

    update(dt) {
      if (!this.alive) {
        return;
      }

      this.x += this.direction * this.speed * dt;

      if (this.x <= this.minX) {
        this.x = this.minX;
        this.direction = 1;
      } else if (this.x >= this.maxX) {
        this.x = this.maxX;
        this.direction = -1;
      }
    }
  }

  class Collectible {
    constructor(definition, config) {
      this.x = definition.x;
      this.y = definition.y;
      this.value = definition.value;
      this.radius = config.radius;
      this.hiddenPath = Boolean(definition.hiddenPath);
      this.collected = false;
    }
  }

  class GoalPortal {
    constructor(config, position) {
      this.x = position.x;
      this.y = position.y;
      this.w = config.width;
      this.h = config.height;
    }

    get bounds() {
      return { x: this.x, y: this.y, w: this.w, h: this.h };
    }
  }

  function moveTowards(current, target, maxDelta) {
    if (current < target) {
      return Math.min(current + maxDelta, target);
    }
    if (current > target) {
      return Math.max(current - maxDelta, target);
    }
    return target;
  }

  function isSolid(platform) {
    return platform.type !== "hazard";
  }

  function resolveHorizontal(player, platforms) {
    const bounds = player.bounds;
    for (const platform of platforms) {
      if (!isSolid(platform)) {
        continue;
      }
      if (!rectsOverlap(bounds, platform)) {
        continue;
      }

      if (player.vx > 0) {
        player.x = platform.x - player.width;
      } else if (player.vx < 0) {
        player.x = platform.x + platform.w;
      }
      player.vx = 0;
      bounds.x = player.x;
    }
  }

  function resolveVertical(player, platforms) {
    const bounds = player.bounds;
    for (const platform of platforms) {
      if (!isSolid(platform)) {
        continue;
      }
      if (!rectsOverlap(bounds, platform)) {
        continue;
      }

      if (player.vy > 0) {
        player.y = platform.y - player.height;
        player.vy = 0;
        player.grounded = true;
      } else if (player.vy < 0) {
        player.y = platform.y + platform.h;
        player.vy = 0;
      }
      bounds.y = player.y;
    }
  }

  window.GameEntities = {
    Player,
    Enemy,
    Collectible,
    GoalPortal,
    rectsOverlap
  };
}());
