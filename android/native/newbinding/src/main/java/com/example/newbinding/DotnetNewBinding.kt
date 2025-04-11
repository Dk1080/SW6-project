package com.example.newbinding

import android.content.Context
import androidx.health.connect.client.HealthConnectClient
import androidx.health.connect.client.aggregate.AggregationResultGroupedByDuration
import androidx.health.connect.client.records.StepsRecord
import androidx.health.connect.client.records.metadata.Device
import androidx.health.connect.client.records.metadata.Metadata
import androidx.health.connect.client.request.AggregateGroupByDurationRequest
import androidx.health.connect.client.request.AggregateRequest
import androidx.health.connect.client.time.TimeRangeFilter
import java.time.Duration
import java.time.Instant
import java.time.LocalDate
import java.time.LocalDateTime
import java.time.ZoneOffset
import java.time.temporal.ChronoUnit

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

            println(StepsRecord.toString())

            healthConnectClient.insertRecords(listOf(stepsRecord))
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
    suspend fun aggregateStepsIntoHours(
        startTime: LocalDateTime,
        endTime: LocalDateTime
    ):  List<DotnetStepDTO>{
        try {
            val response =
                healthConnectClient.aggregateGroupByDuration(
                    AggregateGroupByDurationRequest(
                        metrics = setOf(StepsRecord.COUNT_TOTAL),
                        timeRangeFilter = TimeRangeFilter.between(startTime, endTime),
                        timeRangeSlicer = Duration.ofHours(1)
                    )
                )

            val returnList = mutableListOf<DotnetStepDTO>()
            for (item in response) {
                // Round start and end times to the nearest hour
                val roundedStartT = item.startTime.truncatedTo(ChronoUnit.HOURS)
                val roundedEndT = item.endTime.truncatedTo(ChronoUnit.HOURS)

                val result = item.result[StepsRecord.COUNT_TOTAL]
                if (result != null) {
                    returnList.add(DotnetStepDTO(roundedStartT, roundedEndT, result))
                }
                println("Rounded StartTime: $roundedStartT, Rounded EndTime: $roundedEndT, Result: $result")
            }





            return returnList;
        } catch (e: Exception) {
            // Run error handling here
            return emptyList<DotnetStepDTO>()
        }

    }


    data class DotnetStepDTO(
        val startTime: Instant,
        val endTime: Instant,
        val stepCount: Long
    )

}