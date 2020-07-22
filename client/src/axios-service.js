
const axios = require('axios');
const base_url = //'https://pokeapi.co/api/v2/';
'http://localhost:5000/';
const pagination = "page='5'";


export function getElements(type, callback){
  axios.get(base_url + type)
  .then(function (res) {
      callback(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      callback(error, null);
  })
}

export function addElements(type,params, callback){
  axios.post(base_url + type, params ) 
  .then(function (res) {
      callback(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      callback(error, null);
  })
}

export function updateElement(type,id,params, callback){
  axios.put(base_url + type +'/'+ id, params ) 
  .then(function (res) {
      console.log(res.data);
      callback(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      callback(error, null);
  })
}

export function getElement(type,id, callback){
  axios.get(base_url + type + id ) 
  .then(function (res) {
      console.log(res.data);
      callback(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      callback(error, null);
  })
}

export function getPropertiesById(type, id, cb){
  axios.get(base_url+ type + `/` + id)
  .then(function (res) {
      cb(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      cb(error, null);
  })
}

export function getPropertiesByParams(type, paramName, param, cb){
  axios.get(base_url+ type + `?` + paramName + "=" + param)
  .then(function (res) {
      cb(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      cb(error, null);
  })
}

export function addProperty(type, params, token, cb){
  axios.post(base_url + type + `/add`, params, {headers: { Authorization: "Bearer " + token }}) 
  .then(function (res) {
      console.log(res.data);
      cb(null, res.data);
  })
  .catch(function (error) {
      console.log(error);
      cb(error, null);
  })
}
