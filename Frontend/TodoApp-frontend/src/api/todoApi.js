import axios from "axios";

const api = axios.create({
  baseURL: "http://localhost:5272/api",
});

export const fetchTodos = () => api.get("/todo").then((res) => res.data);

export const fetchTodoById = (id) =>
  api.get(`/todo/${id}`).then((res) => res.data);

export const createTodo = (todo) =>
  api.post("/todo", todo).then((res) => res.data);

export const updateTodo = (id, data) =>
  api.put(`/todo/${id}`, data).then((res) => res.data);

export const completeTodo = (id) =>
  api.put(`/todo/${id}/complete`).then((res) => res.data);

export const deleteTodo = (id) => api.delete(`/todo/${id}`);
