import csv
import tempfile
import unittest
from pathlib import Path

from scripts.report_field_telemetry import nearest_rank, read_samples, summarize


class TelemetryReportTests(unittest.TestCase):
    COLUMNS = [
        "field_id", "field_length", "field_width", "kick_kind", "ball_path",
        "net_forward_progress", "contacts", "path_field_ratio", "outcome"
    ]

    def write_csv(self, directory, rows):
        path = Path(directory) / "playtest.csv"
        with path.open("w", encoding="utf-8", newline="") as output:
            writer = csv.DictWriter(output, fieldnames=self.COLUMNS)
            writer.writeheader()
            writer.writerows(rows)
        return path

    def row(self, **overrides):
        base = {
            "field_id": "lab-tactical-42x27", "field_length": "42",
            "field_width": "27", "kick_kind": "Shot", "ball_path": "21",
            "net_forward_progress": "6", "contacts": "2",
            "path_field_ratio": "0.5", "outcome": "Settled",
        }
        base.update(overrides)
        return base

    def test_group_statistics_are_not_mix_of_pass_and_shot(self):
        with tempfile.TemporaryDirectory() as folder:
            path = self.write_csv(folder, [
                self.row(), self.row(ball_path="0", path_field_ratio="0", contacts="0",
                                     outcome="OutOfBounds"),
                self.row(kick_kind="ControlledPass", ball_path="4.2",
                         path_field_ratio="0.1", contacts="1"),
            ])
            samples = read_samples([path])
            groups = summarize(samples, min_samples=3)
            self.assertEqual(len(groups), 2)
            shot = next(x for x in groups if x["kick_kind"] == "Shot")
            self.assertEqual(shot["samples"], 2)
            self.assertEqual(shot["ball_path_median"], 10.5)
            self.assertEqual(shot["zero_contact_share"], 0.5)
            self.assertEqual(shot["ball_out_share"], 0.5)
            self.assertTrue(shot["exploratory_only"])
            self.assertEqual(nearest_rank([1, 2, 3, 4], .95), 4)

    def test_invalid_ratio_and_nonfinite_values_are_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = self.write_csv(folder, [
                self.row(path_field_ratio="NaN")
            ])
            with self.assertRaises(ValueError):
                read_samples([path])
            path = self.write_csv(folder, [
                self.row(ball_path="22")
            ])
            with self.assertRaises(ValueError):
                read_samples([path])

    def test_chip_samples_are_supported(self):
        with tempfile.TemporaryDirectory() as folder:
            path = self.write_csv(folder, [self.row(kick_kind="Chip")])
            parsed = read_samples([path])
            self.assertEqual(parsed[0]["kick"], "Chip")
            self.assertEqual(len(summarize(parsed)), 1)

    def test_missing_csv_columns_rejected(self):
        with tempfile.TemporaryDirectory() as folder:
            path = Path(folder) / "broken.csv"
            path.write_text("field_id,ball_path\nabc,1\n", encoding="utf-8")
            with self.assertRaises(ValueError):
                read_samples([path])


if __name__ == "__main__":
    unittest.main()
