package com.example.newbinding

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.aggregate.AggregateMetric
import androidx.health.connect.client.aggregate.AggregationResultGroupedByDuration
import androidx.health.connect.client.impl.platform.records.toAggregationType
import androidx.health.connect.client.records.ActiveCaloriesBurnedRecord
import androidx.health.connect.client.records.DistanceRecord
import androidx.health.connect.client.records.ElevationGainedRecord
import androidx.health.connect.client.records.FloorsClimbedRecord
import androidx.health.connect.client.records.HeartRateRecord
import androidx.health.connect.client.records.HeartRateVariabilityRmssdRecord
import androidx.health.connect.client.records.HeightRecord
import androidx.health.connect.client.records.RespiratoryRateRecord
import androidx.health.connect.client.records.RestingHeartRateRecord
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.records.TotalCaloriesBurnedRecord
import androidx.health.connect.client.records.Vo2MaxRecord
import androidx.health.connect.client.records.WeightRecord
import androidx.health.connect.client.records.WheelchairPushesRecord
import androidx.health.connect.client.records.metadata.Device
import androidx.health.connect.client.records.metadata.Metadata
import androidx.health.connect.client.request.AggregateGroupByDurationRequest
import androidx.health.connect.client.request.AggregateRequest
import androidx.health.connect.client.time.TimeRangeFilter
import androidx.health.connect.client.units.Energy
import androidx.health.connect.client.units.Length
import androidx.health.connect.client.units.Mass
import java.time.Duration
import java.time.Instant
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.ZoneOffset
import java.time.temporal.ChronoUnit
import kotlin.reflect.KClass

class DotnetNewBinding(private val Context : Context) {

    private val healthConnectClient = HealthConnectClient.getOrCreate(Context)

    companion object {
        @JvmStatic
        fun getString(myString: String): String {
            return "$myString from kotlin!"
        }
    }

    suspend fun insertSteps(Steps: Int) {
        val endTime = Instant.now()
        val startTime = endTime.minus(Duration.ofMinutes(15))
        try {
            println("dssdfdsfsdf.com")

            val stepsRecord = StepsRecord(
                count = Steps.toLong(),
                startTime = startTime,
                endTime = endTime,
                startZoneOffset = ZoneOffset.UTC,
                endZoneOffset = ZoneOffset.UTC,
            )

            val wheelchairPushesRecord = WheelchairPushesRecord(
                count = Steps.toLong(),
                startTime = startTime,
                endTime = endTime,
                startZoneOffset = ZoneOffset.UTC,
                endZoneOffset = ZoneOffset.UTC
            )

            println(StepsRecord.toString())

            healthConnectClient.insertRecords(listOf(stepsRecord))
            healthConnectClient.insertRecords(listOf(wheelchairPushesRecord))
        } catch (e: Exception) {
            // Run error handling here
            e.printStackTrace() // Log the exception
            println("Error inserting steps: ${e.message}")
        }
    }


    suspend fun aggregateSteps(
        startTime: Long,
        endTime: Long
    ): Long? {
        try {
            val response = healthConnectClient.aggregate(
                AggregateRequest(
                    metrics = setOf(StepsRecord.COUNT_TOTAL),
                    //Convert the C# time to java time.
                    timeRangeFilter = TimeRangeFilter.between(Instant.ofEpochMilli(startTime), Instant.ofEpochMilli(endTime))
                )
            )
            // The result may be null if no data is available in the time range
            val stepCount = response[StepsRecord.COUNT_TOTAL]
            return stepCount
        } catch (e: Exception) {
            // Run error handling here
            println("There was an error")
            return null;
        }
    }


    //Get step data for the last 30 days
    suspend fun getAllData(
        startTime: LocalDateTime,
        endTime: LocalDateTime
    ): List<DotnetStepDTO> {
        val returnList = mutableListOf<DotnetStepDTO>()

        for ((metricString, metric) in metricMapping) {
            var timeRange = Duration.ZERO
            if(metricString in hourlyDataTypes){
                timeRange = Duration.ofHours(1)
            }else if (metricString in dailyDataTypes){
                timeRange = Duration.ofDays(1)
            }

            try {
                val response = healthConnectClient.aggregateGroupByDuration(
                    AggregateGroupByDurationRequest(
                        metrics = setOf(metric),
                        timeRangeFilter = TimeRangeFilter.between(startTime, endTime),
                        timeRangeSlicer = timeRange  
                    )
                )

                for (item in response) {
                    val roundedStartT = item.startTime.truncatedTo(ChronoUnit.HOURS)
                    val roundedEndT = item.endTime.truncatedTo(ChronoUnit.HOURS)
                    val result = item.result.get(metric)

                    val dataCount = when (result) {
                        is Number -> result.toDouble()
                        is Energy -> result.inKilocalories
                        is Length -> result.inMeters
                        is Mass -> result.inKilograms
                        else -> null
                    }

                    if (dataCount != null) {
                        returnList.add(
                            DotnetStepDTO(roundedStartT, roundedEndT, dataCount, metricString)
                        )
                        println("Metric: $metric | Start: $roundedStartT | End: $roundedEndT | Data: $dataCount")
                    }
                }

            } catch (e: Exception) {
                println("Error fetching metric $metric: ${e.message}")
            }
        }

        return returnList
    }

    private val metricMapping = mapOf(
        "ActiveCaloriesBurnedRecord" to ActiveCaloriesBurnedRecord.ACTIVE_CALORIES_TOTAL,
        "TotalCaloriesBurnedRecord" to TotalCaloriesBurnedRecord.ENERGY_TOTAL,
        "DistanceRecord" to DistanceRecord.DISTANCE_TOTAL,
        "ElevationGainedRecord" to ElevationGainedRecord.ELEVATION_GAINED_TOTAL,
        "FloorsClimbedRecord" to FloorsClimbedRecord.FLOORS_CLIMBED_TOTAL,
        "HeartRateRecord" to HeartRateRecord.BPM_AVG,
        "HeightRecord" to HeightRecord.HEIGHT_MAX,
        "RestingHeartRateRecord" to RestingHeartRateRecord.BPM_AVG,
        "StepsRecord" to StepsRecord.COUNT_TOTAL,
        "WeightRecord" to WeightRecord.WEIGHT_AVG,
        "WheelchairPushesRecord" to WheelchairPushesRecord.COUNT_TOTAL
    )

    val hourlyDataTypes = setOf(
        "StepsRecord",
        "HeartRateRecord",
        "RestingHeartRateRecord",
        "ActiveCaloriesBurnedRecord",
        "WheelchairPushesRecord"
    )

    val dailyDataTypes = setOf(
        "TotalCaloriesBurnedRecord",
        "DistanceRecord",
        "ElevationGainedRecord",
        "FloorsClimbedRecord",
        "HeightRecord",
        "WeightRecord",
    )


    data class DotnetStepDTO(
        val startTime: Instant,
        val endTime: Instant,
        val dataCount: Double,
        val metricName: String
    )

}