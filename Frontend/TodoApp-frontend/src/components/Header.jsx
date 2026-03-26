import { motion } from "framer-motion";

export default function Header({ totalCount, completedCount }) {
  const percentage = totalCount > 0 ? Math.round((completedCount / totalCount) * 100) : 0;

  return (
    <motion.header
      initial={{ opacity: 0 }}
      animate={{ opacity: 1 }}
      transition={{ duration: 0.6 }}
    >
      <h1 className="text-3xl font-semibold text-stone-900 tracking-tight">
        Mes tâches
      </h1>

      <div className="flex items-center gap-3 mt-3">
        <div className="flex-1 h-1.5 bg-stone-200 rounded-full overflow-hidden">
          <motion.div
            className="h-full bg-indigo-500 rounded-full"
            initial={{ width: 0 }}
            animate={{ width: `${percentage}%` }}
            transition={{ duration: 0.6, ease: [0.25, 0.46, 0.45, 0.94] }}
          />
        </div>
        <span className="text-xs font-medium text-stone-400 tabular-nums whitespace-nowrap">
          {totalCount === 0
            ? "0 tâche"
            : `${completedCount}/${totalCount}`}
        </span>
      </div>
    </motion.header>
  );
}
