import { useState } from "react";
import { motion, AnimatePresence } from "framer-motion";
import { Check, Trash2, Pencil, X, Check as SaveIcon } from "lucide-react";
import { useCompleteTodo, useDeleteTodo, useUpdateTodo } from "../hooks/useTodos";
import { useTodoStore } from "../store/todoStore";

const itemVariants = {
  hidden: { opacity: 0, y: 8 },
  visible: {
    opacity: 1,
    y: 0,
    transition: { duration: 0.25, ease: [0.25, 0.46, 0.45, 0.94] },
  },
  exit: {
    opacity: 0,
    height: 0,
    marginTop: 0,
    marginBottom: 0,
    paddingTop: 0,
    paddingBottom: 0,
    transition: { duration: 0.2, ease: "easeInOut" },
  },
};

export default function TodoItem({ todo }) {
  const { editingId, setEditingId, clearEditing } = useTodoStore();
  const [editTitle, setEditTitle] = useState(todo.title);

  const completeMutation = useCompleteTodo();
  const deleteMutation = useDeleteTodo();
  const updateMutation = useUpdateTodo();

  const isEditing = editingId === todo.id;

  const handleComplete = () => {
    if (todo.isCompleted) return;
    completeMutation.mutate(todo.id);
  };

  const handleDelete = () => {
    deleteMutation.mutate(todo.id);
  };

  const startEditing = () => {
    setEditTitle(todo.title);
    setEditingId(todo.id);
  };

  const cancelEditing = () => {
    clearEditing();
    setEditTitle(todo.title);
  };

  const handleSave = () => {
    const trimmed = editTitle.trim();
    if (!trimmed || trimmed === todo.title) {
      cancelEditing();
      return;
    }
    updateMutation.mutate(
      { id: todo.id, data: { title: trimmed } },
      { onSuccess: () => clearEditing() }
    );
  };

  const handleKeyDown = (e) => {
    if (e.key === "Enter") handleSave();
    if (e.key === "Escape") cancelEditing();
  };

  return (
    <motion.div
      variants={itemVariants}
      initial="hidden"
      animate="visible"
      exit="exit"
      className={`group flex items-center gap-3 px-3.5 py-3 rounded-lg transition-colors duration-150 overflow-hidden
        ${todo.isCompleted ? "opacity-60" : "hover:bg-stone-50"}`}
    >
      <button
        onClick={handleComplete}
        disabled={todo.isCompleted}
        className={`flex-shrink-0 w-[18px] h-[18px] rounded-full border-[1.5px] flex items-center justify-center cursor-pointer transition-all duration-200
          ${todo.isCompleted
            ? "bg-indigo-500 border-indigo-500"
            : "border-stone-300 hover:border-indigo-400"
          }`}
      >
        <AnimatePresence>
          {todo.isCompleted && (
            <motion.div
              initial={{ scale: 0 }}
              animate={{ scale: 1 }}
              transition={{ type: "spring", stiffness: 500, damping: 25 }}
            >
              <Check className="w-2.5 h-2.5 text-white" strokeWidth={3} />
            </motion.div>
          )}
        </AnimatePresence>
      </button>

      <div className="flex-1 min-w-0">
        {isEditing ? (
          <input
            type="text"
            value={editTitle}
            onChange={(e) => setEditTitle(e.target.value)}
            onKeyDown={handleKeyDown}
            maxLength={200}
            autoFocus
            className="w-full px-2.5 py-1 text-sm border border-indigo-200 rounded-md bg-white outline-none
                       focus:ring-2 focus:ring-indigo-100 transition-all duration-150"
          />
        ) : (
          <span
            className={`text-sm text-left block truncate transition-all duration-200
              ${todo.isCompleted ? "line-through text-stone-400" : "text-stone-700"}`}
          >
            {todo.title}
          </span>
        )}
      </div>

      <div className="flex items-center gap-0.5">
        {isEditing ? (
          <>
            <button
              onClick={handleSave}
              className="p-1.5 text-indigo-500 hover:bg-indigo-50 rounded-md cursor-pointer transition-colors duration-150"
            >
              <SaveIcon className="w-3.5 h-3.5" />
            </button>
            <button
              onClick={cancelEditing}
              className="p-1.5 text-stone-400 hover:bg-stone-100 rounded-md cursor-pointer transition-colors duration-150"
            >
              <X className="w-3.5 h-3.5" />
            </button>
          </>
        ) : (
          <>
            {!todo.isCompleted && (
              <button
                onClick={startEditing}
                className="p-1.5 text-stone-300 hover:text-indigo-500 hover:bg-indigo-50 rounded-md
                           opacity-0 group-hover:opacity-100 cursor-pointer transition-all duration-150"
              >
                <Pencil className="w-3.5 h-3.5" />
              </button>
            )}
            <button
              onClick={handleDelete}
              className="p-1.5 text-stone-300 hover:text-red-500 hover:bg-red-50 rounded-md
                         opacity-0 group-hover:opacity-100 cursor-pointer transition-all duration-150"
            >
              <Trash2 className="w-3.5 h-3.5" />
            </button>
          </>
        )}
      </div>
    </motion.div>
  );
}
