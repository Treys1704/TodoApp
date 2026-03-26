import { motion } from "framer-motion";
import { useTodoStore } from "../store/todoStore";

const filters = [
  { key: "all", label: "Toutes" },
  { key: "active", label: "Actives" },
  { key: "completed", label: "Terminées" },
];

export default function FilterBar() {
  const { filter, setFilter } = useTodoStore();

  return (
    <div className="flex gap-1 p-1 bg-stone-100 rounded-lg mb-4">
      {filters.map((f) => (
        <button
          key={f.key}
          onClick={() => setFilter(f.key)}
          className={`relative flex-1 py-1.5 px-3 text-xs font-medium rounded-md cursor-pointer transition-colors duration-200
            ${filter === f.key ? "text-indigo-600" : "text-stone-400 hover:text-stone-600"}`}
        >
          {filter === f.key && (
            <motion.div
              layoutId="activeFilter"
              className="absolute inset-0 bg-white rounded-md shadow-xs"
              transition={{ type: "spring", stiffness: 500, damping: 35 }}
            />
          )}
          <span className="relative z-10">{f.label}</span>
        </button>
      ))}
    </div>
  );
}
