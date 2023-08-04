package com.example.demo;

import androidx.appcompat.app.AppCompatActivity;
//
import android.os.Build;
import android.os.Bundle;
import android.os.VibrationAttributes;
import android.os.VibrationEffect;
import android.os.Vibrator;
import android.os.VibratorManager;
//
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.CompoundButton;
import android.widget.RadioGroup;
import android.widget.SeekBar;
import android.widget.TextView;

public class MainActivity extends AppCompatActivity {
    //
    private CheckBox checkBoxObsoleteType;
    private CheckBox checkBoxLegacyType;
    //
    private SeekBar seekBarDuration;
    private TextView tvDurationLabel;
    private SeekBar seekBarAmplitude;
    private TextView tvAmplitudeLabel;
    private RadioGroup radioGroupVibrationType;

    //
    private Button btnVibrate;
    private TextView tvDeviceLabel;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main);

        //
        this.checkBoxObsoleteType = this.findViewById(R.id.checkBoxObsoleteType);
        this.checkBoxLegacyType = this.findViewById(R.id.checkBoxLegacyType);

        //
        this.seekBarDuration = this.findViewById(R.id.seekBarDuration);
        this.tvDurationLabel = this.findViewById(R.id.tvDurationLabel);
        this.seekBarAmplitude = this.findViewById(R.id.seekBarAmplitude);
        this.tvAmplitudeLabel = this.findViewById(R.id.tvAmplitudeLabel);
        this.radioGroupVibrationType = this.findViewById(R.id.radioGroupVibrationType);

        this.btnVibrate = this.findViewById(R.id.btnVibrate);
        this.tvDeviceLabel = this.findViewById(R.id.tvDeviceLabel);

        //
        this.seekBarDuration.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                float seconds = (float) progress / 100.0f;
                if(seconds < 0.01f) {
                    seconds = 0.01f;
                }
                String durationText = String.format("%.2f second", seconds);
                tvDurationLabel.setText(durationText);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) { }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) { }
        });

        this.seekBarAmplitude.setOnSeekBarChangeListener(new SeekBar.OnSeekBarChangeListener() {
            @Override
            public void onProgressChanged(SeekBar seekBar, int progress, boolean fromUser) {
                int amplitude = progress;
                if(amplitude < 10) {
                    amplitude = 10;
                }
                String amplitudeText = String.format("Amplitude %d", amplitude);
                tvAmplitudeLabel.setText(amplitudeText);
            }

            @Override
            public void onStartTrackingTouch(SeekBar seekBar) { }

            @Override
            public void onStopTrackingTouch(SeekBar seekBar) { }
        });

        //
        this.checkBoxObsoleteType.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean checked) {
                if(checked) {
                    seekBarAmplitude.setEnabled(false);
                }
                else
                {
                    seekBarAmplitude.setEnabled(true);
                }
            }
        });
        this.checkBoxLegacyType.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean checked) {
                if(checked) {
                    checkBoxObsoleteType.setEnabled(true);
                    radioGroupVibrationType.setEnabled(false);
                    for (int i = 0; i < radioGroupVibrationType.getChildCount(); i++) {
                        radioGroupVibrationType.getChildAt(i).setEnabled(false);
                    }
                }
                else
                {
                    checkBoxObsoleteType.setEnabled(false);
                    radioGroupVibrationType.setEnabled(true);
                    for (int i = 0; i < radioGroupVibrationType.getChildCount(); i++) {
                        radioGroupVibrationType.getChildAt(i).setEnabled(true);
                    }
                }
            }
        });

        //
        this.btnVibrate.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {
                int milliseconds = seekBarDuration.getProgress() * 10;  // 将进度值映射到 0.01 到 1 秒之间
                int amplitude = seekBarAmplitude.getProgress();
                if(milliseconds < 10) { milliseconds = 10; }
                if(amplitude < 10) { amplitude = 10; }
                else if(amplitude >= 255) { amplitude = 0; }

                boolean alarm = false;
                int selectedId = radioGroupVibrationType.getCheckedRadioButtonId();
                if (selectedId == R.id.radioAlarm) {
                    alarm = true;
                } else if (selectedId == R.id.radioRingtone) {
                }

                tvDeviceLabel.setText(String.format("Device SDK %d, Android %s. milliseconds %d, amplitude %d",
                        Build.VERSION.SDK_INT, Build.VERSION.RELEASE,
                        milliseconds, amplitude));

                //
                vibrate(milliseconds, amplitude, alarm,
                        checkBoxLegacyType.isChecked(),
                        checkBoxObsoleteType.isChecked());
            }
        });

        //
        this.tvDeviceLabel.setText(String.format("Device SDK %d, Android %s", Build.VERSION.SDK_INT, Build.VERSION.RELEASE));
        this.checkBoxLegacyType.setChecked(false);
    }

    protected void vibrate(long milliseconds, boolean light)
    {
        this.vibrate(milliseconds, 40, false, true, false);
    }

    protected void vibrate(long milliseconds, int amplitude, boolean alarm, boolean legacy, boolean obsolete)
    {
        Vibrator vibrator = (Vibrator) this.getSystemService(VIBRATOR_SERVICE);
        if(Build.VERSION.SDK_INT >= 31) {
            VibratorManager manager = (VibratorManager) this.getSystemService(VIBRATOR_MANAGER_SERVICE);
            vibrator = manager.getDefaultVibrator();
        }

        if(Build.VERSION.SDK_INT >= 26 && vibrator.hasAmplitudeControl() && !obsolete) {
            if(amplitude == 0) {
                amplitude = VibrationEffect.DEFAULT_AMPLITUDE;
            }

            VibrationEffect effect = VibrationEffect.createOneShot(milliseconds, amplitude);
            if(!legacy && Build.VERSION.SDK_INT >= 33) {
                int usage = VibrationAttributes.USAGE_RINGTONE;
                if(alarm) {
                    usage = VibrationAttributes.USAGE_ALARM;
                }
                VibrationAttributes attributes = VibrationAttributes.createForUsage(usage);
                vibrator.vibrate(effect, attributes);
            }
            else {
                vibrator.vibrate(effect);
            }
        }
        else {
            vibrator.vibrate(milliseconds);
        }
    }
}