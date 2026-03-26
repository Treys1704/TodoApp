import { create } from "zustand";

export const useTodoStore = create((set) => ({
  filter: "all",
  setFilter: (filter) => set({ filter }),

  editingId: null,
  setEditingId: (id) => set({ editingId: id }),
  clearEditing: () => set({ editingId: null }),
}));
