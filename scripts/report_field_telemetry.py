#!/usr/bin/env python3
"""Aggregate BFC FormationPlaytestLab CSV samples. Informational, not approval."""
from __future__ import annotations

import argparse
import csv
import json
import math
import statistics
from collections import defaultdict
from pathlib import Path

COLUMNS = {
    "field_id", "field_length", "field_width", "kick_kind",
    "ball_path", "net_forward_progress", "contacts",
    "path_field_ratio", "outcome",
}


def read_samples(paths: list[Path]) -> list[dict]:
    rows = []
    for path in paths:
        with path.open("r", encoding="utf-8-sig", newline="") as stream:
            reader = csv.DictReader(stream)
            if not reader.fieldnames or not COLUMNS.issubset(reader.fieldnames):
                raise ValueError(f"{path}: CSV missing required columns: {sorted(COLUMNS - set(reader.fieldnames or []))}")
            for number, row in enumerate(reader, start=2):
                label = f"{path}:{number}"
                try:
                    length = float(row["field_length"])
                    width = float(row["field_width"])
                    traveled = float(row["ball_path"])
                    advance = float(row["net_forward_progress"])
                    normalized = float(row["path_field_ratio"])
                    contacts = int(row["contacts"])
                except (TypeError, ValueError, OverflowError) as error:
                    raise ValueError(f"{label}: invalid numeric value") from error
                if not all(math.isfinite(x) for x in (length, width, traveled, advance, normalized)):
                    raise ValueError(f"{label}: nonfinite numeric value")
                if length <= 0 or width <= 0 or traveled < 0 or normalized < 0 or contacts < 0:
                    raise ValueError(f"{label}: invalid nonnegative/positive constraints")
                if not math.isclose(traveled / length, normalized, rel_tol=1e-4, abs_tol=1e-5):
                    raise ValueError(f"{label}: path_field_ratio inconsistent with ball_path")
                if not row["field_id"] or row["kick_kind"] not in {"Shot", "ControlledPass"}:
                    raise ValueError(f"{label}: invalid field or action")
                if row["outcome"] not in {"Settled", "Goal", "OutOfBounds"}:
                    raise ValueError(f"{label}: invalid outcome")
                rows.append({
                    "field": row["field_id"],
                    "kick": row["kick_kind"],
                    "field_length": length,
                    "field_width": width,
                    "path": traveled,
                    "forward": advance,
                    "path_ratio": normalized,
                    "contacts": contacts,
                    "outcome": row["outcome"],
                })
    return rows


def nearest_rank(values: list[float], percentile: float) -> float:
    if not values:
        raise ValueError("percentile requires nonempty sample")
    values = sorted(values)
    return values[max(0, math.ceil(len(values) * percentile) - 1)]


def summarize(rows: list[dict], min_samples: int = 10) -> list[dict]:
    if min_samples < 1:
        raise ValueError("min_samples must be positive")
    groups = defaultdict(list)
    for row in rows:
        groups[(row["field"], row["kick"], row["field_length"], row["field_width"])].append(row)

    result = []
    for (field, kick, length, width), subset in sorted(groups.items()):
        n = len(subset)
        result.append({
            "field": field,
            "kick_kind": kick,
            "field_length": length,
            "field_width": width,
            "samples": n,
            "exploratory_only": n < min_samples,
            "ball_path_median": round(statistics.median(x["path"] for x in subset), 4),
            "ball_path_p95": round(nearest_rank([x["path"] for x in subset], 0.95), 4),
            "path_to_field_length_median": round(statistics.median(x["path_ratio"] for x in subset), 4),
            "net_forward_median": round(statistics.median(x["forward"] for x in subset), 4),
            "zero_contact_share": round(sum(x["contacts"] == 0 for x in subset) / n, 4),
            "ball_out_share": round(sum(x["outcome"] == "OutOfBounds" for x in subset) / n, 4),
            "goal_share": round(sum(x["outcome"] == "Goal" for x in subset) / n, 4),
        })
    return result


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("csv_files", nargs="+", type=Path, help="CSV files exported with C in playtest")
    parser.add_argument("--json-out", type=Path, help="Optional output path for aggregated metrics")
    parser.add_argument("--min-samples", type=int, default=10)
    args = parser.parse_args()
    try:
        summaries = summarize(read_samples(args.csv_files), args.min_samples)
    except (OSError, ValueError) as error:
        parser.error(str(error))
    print("BFC TELEMETRIA - PENDENTE DE VALIDACAO (metricas observacionais)")
    if not summaries:
        print("No completed samples.")
        return 1
    for item in summaries:
        print(f"{item['field']} | {item['kick_kind']} | n={item['samples']} "
              f"mediana={item['ball_path_median']:.2f} P95={item['ball_path_p95']:.2f} "
              f"caminho/campo={item['path_to_field_length_median']:.3f} "
              f"avanco={item['net_forward_median']:.2f} "
              f"sem_contato={item['zero_contact_share']:.1%} "
              f"fora={item['ball_out_share']:.1%} "
              f"amostra_pequena={'SIM' if item['exploratory_only'] else 'NAO'}")
    if args.json_out:
        args.json_out.parent.mkdir(parents=True, exist_ok=True)
        args.json_out.write_text(
            json.dumps({"status": "PENDENTE DE VALIDACAO", "groups": summaries},
                       ensure_ascii=False, indent=2) + "\n",
            encoding="utf-8")
        print(f"JSON saved: {args.json_out}")
    return 0


if __name__ == "__main__":
    raise SystemExit(main())
