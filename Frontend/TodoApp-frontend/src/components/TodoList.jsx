import { motion, AnimatePresence } from "framer-motion";
import { useTodosQuery } from "../hooks/useTodos";
import { useTodoStore } from "../store/todoStore";
import TodoItem from "./TodoItem";
import EmptyState from "./EmptyState";
import { Loader2 } from "lucide-react";

const listVariants = {
  hidden: {},
  visible: {
    transition: { staggerChildren: 0.04 },
  },
};

export default function TodoList() {
  const { data: todos, isLoading, isError } = useTodosQuery();
  const filter = useTodoStore((s) => s.filter);

  if (isLoading) {
    return (
      <div className="flex items-center justify-center py-16">
        <Loader2 className="w-5 h-5 text-indigo-400 animate-spin" />
      </div>
    );
  }

  if (isError) {
    return (
      <div className="text-center py-16 px-4">
        <p className="text-red-400 text-sm">
          Impossible de charger les tâches. Vérifiez que le backend est démarré.
        </p>
      </div>
    );
  }

  const filtered = todos.filter((todo) => {
    if (filter === "active") return !todo.isCompleted;
    if (filter === "completed") return todo.isCompleted;
    return true;
  });

  if (filtered.length === 0) {
    return <EmptyState filter={filter} />;
  }

  return (
    <motion.div
      key={filter}
      className="flex flex-col gap-1.5"
      variants={listVariants}
      initial="hidden"
      animate="visible"
    >
      <AnimatePresence initial={false}>
        {filtered.map((todo) => (
          <TodoItem key={todo.id} todo={todo} />
        ))}
      </AnimatePresence>
    </motion.div>
  );
}
