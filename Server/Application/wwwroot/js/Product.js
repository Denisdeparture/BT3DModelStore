import * as THREE from "three";
import { GLTFLoader } from "https://unpkg.com/three@0.165.0/examples/jsm/loaders/GLTFLoader.js";
import { OrbitControls } from 'https://unpkg.com/three@0.165.0/examples/jsm/controls/OrbitControls.js';
let urlsfromstr = document.getElementById("test").innerHTML;

var scene = new THREE.Scene();



var camera = new THREE.PerspectiveCamera(40, window.innerWidth / window.innerHeight, 1, 1000);

var renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });

let loader = new GLTFLoader();

let obj = null;

renderer.setClearColor(0x000000, 0);
renderer.setAnimationLoop(animate);
renderer.setSize(500, 500);

let view = document.getElementById("images-control");

let container_in_view = document.getElementById("canvas-container");

renderer.domElement.setAttribute("id", "Model");

view.insertBefore(renderer.domElement, container_in_view);
const controls = new OrbitControls(camera, renderer.domElement);
controls.minDistance = 5;
controls.maxDistance = 200;
controls.maxPolarAngle = Math.PI / 2;

camera.position.set(15, 20, 30);
const light = new THREE.DirectionalLight(0xFFFFFF, 1);
scene.add(light);
light.position.set(3, 3, 3);

camera.add(new THREE.PointLight(0xffffff, 3, 0, 0));
function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
    camera.lookAt(scene.position)
}
loader.load(urlsfromstr, function (object) {
    obj = object.scene;
    obj.position.set(0, 0, 0);
    scene.add(obj);
});
animate();
