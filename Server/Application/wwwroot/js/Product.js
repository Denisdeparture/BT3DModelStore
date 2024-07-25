import * as THREE from "three";
import { GLTFLoader } from "https://unpkg.com/three@0.165.0/examples/jsm/loaders/GLTFLoader.js";
let urlsfromstr = document.getElementById("test").innerHTML;

var scene = new THREE.Scene();

var camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);

var renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });

const aLight = new THREE.AmbientLight(0x404040, 1.2);

const pLight = new THREE.PointLight(0xFFFFFF, 1.2);

const helper = new THREE.PointLightHelper(pLight);

let loader = new GLTFLoader();

let obj = null;

renderer.setClearColor(0x000000, 0);

renderer.setSize(500, 500);

let view = document.getElementById("images-control");

let container_in_view = document.getElementById("canvas-container");

renderer.domElement.setAttribute("id", "Model");

view.insertBefore(renderer.domElement, container_in_view);


//camera.position.z = 2;
//pLight.position.set(0, 0, -10);

scene.add(aLight);

scene.add(pLight);

scene.add(helper);
function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
    camera.lookAt(scene.position)
}
loader.load(urlsfromstr, function (object) {
    obj = object.scene;
    //obj.position.set(0, 0, 0);
    //obj.rotation.x = 1.3;
    //obj.rotation.y = 1.3;
    scene.add(obj);
});
animate();
