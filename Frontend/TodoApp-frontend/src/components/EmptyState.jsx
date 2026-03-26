import { motion } from "framer-motion";

export default function EmptyState({ filter }) {
  const messages = {
    all: "Aucune tâche pour le moment",
    active: "Toutes les tâches sont terminées",
    completed: "Aucune tâche terminée",
  };

  return (
    <motion.p
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.3 }}
      className="text-center py-12 text-sm text-stone-400"
    >
      {messages[filter]}
    </motion.p>
  );
}
