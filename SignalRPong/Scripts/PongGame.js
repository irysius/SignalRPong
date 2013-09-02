var canvas, stage, preload;
var paddle1Image, paddle2Image, ballImage;
var paddle1Bitmap, paddle2Bitmap, ballBitmap;

var paddle1Y = 0, paddle2Y = 0, ballX = 0, ballY = 0;
var score1 = 0, score2 = 0;
var score1Text, score2Text;
var ballVX = 0; ballVY = 0;
var isPlayer1 = true;

var pongHub;

function initializeGame() {
    canvas = document.getElementById('main-canvas');
    canvas.height = 450;
    canvas.width = 700;
    stage = new createjs.Stage(canvas);
    preload = new createjs.LoadQueue(true);

    var manifest = [
        { id: "backgroundImage", src: "/Content/images/grid.png" },
        { id: "paddle1Image", src: "/Content/images/paddle1.png" },
        { id: "paddle2Image", src: "/Content/images/paddle2.png" },
        { id: "ballImage", src: "/Content/images/ball.png" }
    ];

    preload.addEventListener("complete", prepareAssets);
    preload.loadManifest(manifest);
}

function prepareAssets() {
    paddle1Image = preload.getResult("paddle1Image");
    paddle1Bitmap = new createjs.Bitmap(paddle1Image);

    paddle2Image = preload.getResult("paddle2Image");
    paddle2Bitmap = new createjs.Bitmap(paddle2Image);
    paddle2Bitmap.x = canvas.width - paddle2Image.width;

    ballImage = preload.getResult("ballImage");
    ballBitmap = new createjs.Bitmap(ballImage);
    ballBitmap.regX = ballImage.width / 2;
    ballBitmap.regY = ballImage.height / 2;
    ballBitmap.x = canvas.width / 2;
    ballBitmap.y = canvas.height / 2;

    backgroundImage = preload.getResult("backgroundImage");
    backgroundBitmap = new createjs.Bitmap(backgroundImage);

    score1Text = new createjs.Text("P1: " + score1, "20px sans-serif", "Dark Red");
    score1Text.y = 5;
    score1Text.x = 15;
    score2Text = new createjs.Text("P2: " + score2, "20px sans-serif", "Dark Red");
    score2Text.y = 5;
    score2Text.x = canvas.width - 15 - score2Text.getMeasuredWidth();

    stage.addChild(backgroundBitmap);
    stage.addChild(ballBitmap);
    stage.addChild(paddle2Bitmap);
    stage.addChild(paddle1Bitmap);
    stage.addChild(score1Text);
    stage.addChild(score2Text);
    stage.mouseMoveOutside = true;
    stage.update();
}

function startGame() {
    stage.onMouseMove = movePaddle;
    stage.addEventListener("click", serveBall);
    createjs.Ticker.setInterval(window.requestAnimationFrame);
    createjs.Ticker.addListener(gameLoop);
}

function gameLoop() {
    update();
    draw();
}

function update() {
    ballBitmap.x = ballX;
    ballBitmap.y = ballY;
    paddle1Bitmap.y = paddle1Y;
    paddle2Bitmap.y = paddle2Y;
    score1Text.text = "P1: " + this.score1;
    score2Text.text = "P2: " + this.score2;
}

function draw() {
    stage.update();
}

function movePaddle(evt) {
    var temp = evt.stageY - paddle1Image.height / 2;
    if (temp < 0) {
        temp = 0;
    }
    if (temp > canvas.height - paddle1Image.height) {
        temp = canvas.height - paddle1Image.height;
    }
    console.log("mouse move " + temp);
    if (isPlayer1) {
        paddle1Y = temp;
    } else {
        paddle2Y = temp;
    }
    pongHub.server.updatePaddle(roomId, isPlayer1, temp);
}

function serveBall(evt) {
    pongHub.server.serveBall(roomId, isPlayer1);
}

function updatePositions(p1, p2, bx, by) {
    if (!isPlayer1) {
        paddle1Y = p1;
    } else {
        paddle2Y = p2;
    }
    ballX = bx;
    bally = by;
}

function updateScore (s1, s2) {
    score1 = s1;
    score2 = s2;
};