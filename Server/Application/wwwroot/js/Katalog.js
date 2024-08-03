import * as THREE from "three";
import { GLTFLoader } from "https://unpkg.com/three@0.165.0/examples/jsm/loaders/GLTFLoader.js";


let urlsfromstr = document.getElementById("test").innerHTML;

let array = urlsfromstr.split(" ");

var scene = new THREE.Scene();

var camera = new THREE.PerspectiveCamera(45, window.innerWidth / window.innerHeight, 0.1, 1000);

var renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });

const aLight = new THREE.AmbientLight(0x404040, 1.2);

const pLight = new THREE.PointLight(0xFFFFFF, 1.2);

const helper = new THREE.PointLightHelper(pLight);

let loader = new GLTFLoader();

let obj = null;

renderer.setClearColor(0x000000, 0);
let main = document.getElementById("main-el");
renderer.setSize(1000, 1000);
let ul = document.getElementById("canvas-container");
main.insertBefore(renderer.domElement, ul);

let positions = [];
for (let j = 0; j < array.length; j++)
{
    let el = "info-prod " + j;
    let productinfo = document.getElementById(el);
    let annonymusObjectCord = { x: productinfo.offsetLeft, y: productinfo.offsetTop }
    positions.push(annonymusObjectCord);
}
renderer.domElement.setAttribute("id", "Model");
camera.position.z = 10;

pLight.position.set(1, 1, 53);

scene.add(aLight);

scene.add(pLight);

scene.add(helper);
function animate() {
    requestAnimationFrame(animate);
    renderer.render(scene, camera);
    camera.lookAt(scene.position)
}
for (let k = 0; k < array.length; k++) {
    loader.loadAsync(array[k]).then(object => {
        obj = object.scene;
        obj.position.set(positions[k].x, positions[k].y, - 53)
        obj.rotation.x = 1.3;
        obj.rotation.y = 1.3;
        scene.add(obj);
    });
    animate();
}





