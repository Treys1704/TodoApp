import { useState } from "react";
import { Plus } from "lucide-react";
import { useCreateTodo } from "../hooks/useTodos";

export default function TodoForm() {
  const [title, setTitle] = useState("");
  const createTodo = useCreateTodo();

  const handleSubmit = (e) => {
    e.preventDefault();
    const trimmed = title.trim();
    if (!trimmed) return;

    createTodo.mutate({ title: trimmed }, { onSuccess: () => setTitle("") });
  };

  return (
    <form onSubmit={handleSubmit} className="flex gap-2 mb-5">
      <input
        type="text"
        value={title}
        onChange={(e) => setTitle(e.target.value)}
        placeholder="Nouvelle tâche..."
        maxLength={200}
        className="flex-1 px-3.5 py-2.5 rounded-lg border border-stone-200 bg-stone-50 text-stone-900 text-sm
                   placeholder-stone-400 outline-none
                   focus:bg-white focus:border-indigo-300 focus:ring-2 focus:ring-indigo-50
                   transition-all duration-200"
      />
      <button
        type="submit"
        disabled={createTodo.isPending || !title.trim()}
        className="px-4 py-2.5 bg-indigo-500 text-white rounded-lg font-medium text-sm
                   flex items-center gap-1.5 cursor-pointer
                   hover:bg-indigo-600 active:scale-[0.97]
                   disabled:opacity-30 disabled:cursor-not-allowed
                   transition-all duration-150"
      >
        <Plus className="w-4 h-4" strokeWidth={2.5} />
        <span>Ajouter</span>
      </button>
    </form>
  );
}
